using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPromedyTargets : MonoBehaviour {

	public float timeBetweenRefocus = 0.2f;                 // Approximate time for the camera to refocus.
	public float screenEdge = 4f;							// Space between the top/bottom most target and the screen edge.
	public float minSize = 6.5f;							// The smallest fieldOfView.
	public Transform[] targets;				

	private Camera cam;                         
	private float zoomSpeed;                     
	private Vector3 moveVelocity;               
	private Vector3 desiredPosition;             

	void Start()
	{
		targets = new Transform[2];
		
		var player = FindObjectOfType<PlayerController>().gameObject;
		var boss = FindObjectOfType<BossController>().gameObject;
		targets[0] = player.transform;
		targets[1] = boss.transform;
	}

	void FixedUpdate()
	{
		if (cam == null)
			cam = GetComponentInChildren<Camera>();
		else
		{
			Move();
			Zoom();
		}
	}


	void Move()
	{
		// Find the average position of the targets.
		FindAveragePosition();

		// Smoothly transition to that position.
		transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, timeBetweenRefocus);
	}


	void FindAveragePosition()
	{
		Vector3 averagePos = new Vector3();
		int numTargets = 0;

		// Go through all the targets and add their positions together.
		for (int i = 0; i < targets.Length; i++)
		{
			// If the target isn't active, go on to the next one.
			if (!targets[i].gameObject.activeSelf)
				continue;

			// Add to the average and increment the number of targets in the average.
			averagePos += targets[i].position;
			numTargets++;
		}

		// If there are targets divide the sum of the positions by the number of them to find the average.
		if (numTargets > 0)
			averagePos /= numTargets;

		// Keep the same y value.
		averagePos.y = transform.position.y;

		// The desired position is the average position;
		desiredPosition = averagePos;
	}


	void Zoom()
	{
		// Find the required size based on the desired position and smoothly transition to that size.
		float requiredSize = FindRequiredSize();
		cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, requiredSize, ref zoomSpeed, timeBetweenRefocus);
	}


	float FindRequiredSize()
	{
		// Find the position the camera rig is moving towards in its local space.
		Vector3 desiredLocalPos = transform.InverseTransformPoint(desiredPosition);

		// Start the camera's size calculation at zero.
		float size = 0f;

		// Go through all the targets...
		for (int i = 0; i < targets.Length; i++)
		{
			// ... and if they aren't active continue on to the next target.
			if (!targets[i].gameObject.activeSelf)
				continue;

			// Otherwise, find the position of the target in the camera's local space.
			Vector3 targetLocalPos = transform.InverseTransformPoint(targets[i].position);

			// Find the position of the target from the desired position of the camera's local space.
			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

			// Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
			size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

			// Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
			size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / cam.aspect);
		}

		// Add the edge buffer to the size.
		size += screenEdge;

		// Make sure the camera's size isn't below the minimum.
		size = Mathf.Max(size, minSize);

		return size;
	}
}
