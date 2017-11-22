using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormWander : Enemy {

	GameManager gm;
	public float damage;
	private void Start()
	{
		gm = FindObjectOfType<GameManager>();
	}
	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.layer == 9)
		{
			Instantiate(gm.bloodWorm, head.transform);
			life -= c.gameObject.GetComponent<PlayerBullets>().damage;
		}
		
		//player
		if (c.gameObject.layer == 8)
			c.gameObject.GetComponent<PlayerController>().life -= damage;
	}
	private void Update()
	{
		if (life <= 0)
		{
			gm.enemiesDead++;
			this.gameObject.SetActive(false);
		}
	}
}
