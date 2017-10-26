using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Enemy{

	float _life;
	EnemySpawner enemySpawner;
	private void Start()
	{
		enemySpawner = FindObjectOfType<EnemySpawner>();
		_life = life;
	}
	public override void Initialize()
	{
		var spawners = FindObjectOfType<EnemySpawner>().spawners;
		var index = FindObjectOfType<EnemySpawner>().enemiesSpawned;
		transform.position = spawners[index].transform.position;
		life = 5;
	}

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.layer == 9)
			life -= c.gameObject.GetComponent<PlayerBullets>().damage;
	}
	void Update()
	{
		if (life <= 0)
		{
			enemySpawner.totalEnemies--;
			enemySpawner.enemiesAlive--;
			EnemySpawner.Instance.ReturnWormToPool(this);
		}
	}
}
