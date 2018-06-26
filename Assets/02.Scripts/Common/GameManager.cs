using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // 적 캐릭터가 출현할 위치정보 배열
    public Transform[] spawnPoints;

    // 적 캐릭터의 원본 프리팹을 저장할 변수
    public GameObject enemyPrefab;

    public float spawnEnemyCycle = 5.0f;

    public int maxEnemyCount = 10;

    public bool isGameOver = false;

	// Use this for initialization
	void Start () {
        spawnPoints = GameObject.Find("EnemySpawnPointGroup").GetComponentsInChildren<Transform>();
        
        if(spawnPoints.Length > 0)
        {
            StartCoroutine(GenerateEnemy());
        }
	}

    IEnumerator GenerateEnemy()
    {
        while (!isGameOver)
        {
            int enemyCount = GameObject.FindGameObjectsWithTag("ENEMY").Length;

            if (enemyCount < maxEnemyCount)
            {
                yield return new WaitForSeconds(spawnEnemyCycle);

                int index = Random.Range(0, spawnPoints.Length);
                Instantiate(enemyPrefab, spawnPoints[index].position, spawnPoints[index].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
