using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private static EnemySpawner _instance;
    public static EnemySpawner Instance { get { return _instance; } }
    public List<GameObject> spawners = new List<GameObject>();
    public int waves;
    public int totalEnemies;
    public int enemiesSpawned;
    public GameObject door;
	public bool allSpawnerDeads;
    public int enemiesAlive;
    public Enemy enemyPrefab;
    private Pool<Enemy> enemyPool;

    int enemiesPerWave;
    int wave = 0;
	int currentWave;

    List<int> spawnPerVawe = new List<int>();

    void Awake()
    {
        _instance = this;
        enemyPool = new Pool<Enemy>(totalEnemies, EnemyFactory, Worm.InitializeEnemy, Worm.DisposeEnemy, true);
    }

    void Start()
    {
        Utility.KnuthShuffle<GameObject>(spawners);
        enemiesPerWave = totalEnemies / waves;

        for (int i = 0; i <= waves; i++)
        {
            if (i == 0)
            {
                enemiesPerWave = enemiesPerWave - (enemiesPerWave / 2);
            }

            else
            {
                enemiesPerWave = enemiesPerWave + (enemiesPerWave / 2);
            }

            spawnPerVawe.Add(enemiesPerWave);
        }	
        StartCoroutine(SpawnVawe(spawnPerVawe[wave]));
		wave++;
    }

    void Update()
    {
		if (allSpawnerDeads && enemiesAlive == 0 && door.activeSelf)
        {
            door.SetActive(false);
        }

        if (!allSpawnerDeads&&enemiesAlive == 0 && totalEnemies > 0 && wave < waves)
        {
            enemiesSpawned = 0;
			wave++;
			StartCoroutine(SpawnVawe(spawnPerVawe[wave]));
		
		}
    }

    IEnumerator SpawnVawe(int cantEnemies)
    {
        for (int i = 0; i < cantEnemies; i++)
        {
            enemiesSpawned++;

            if (enemiesSpawned >= spawners.Count)
            {
                Utility.KnuthShuffle<GameObject>(spawners);
                enemiesSpawned = 0;
            }

            enemyPool.GetObjectFromPool();
            enemiesAlive++;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private Enemy EnemyFactory()
    {
        return Instantiate<Enemy>(enemyPrefab);
    }

    public void ReturnBulletToPool(Enemy enemy)
    {
        enemyPool.DisablePoolObject(enemy);
    }
}
