﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region 적 생성 관련 변수
    // 적 캐릭터가 출현할 위치정보 배열
    public Transform[] spawnPoints;

    // 적 캐릭터의 원본 프리팹을 저장할 변수
    public GameObject enemyPrefab;

    public float spawnEnemyCycle = 5.0f;

    public int maxEnemyCount = 10;
    #endregion

    public bool isGameOver = false;

    public static GameManager instance = null;

    #region 총알 오브젝트 풀링 괄련 변수
    public GameObject bulletPrefab;

    // 오브젝트 풀에 풀링할 개수
    public int maxPool = 10;

    public List<GameObject> bulletPool = new List<GameObject>();
    #endregion

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            CreatePool();
        }
    }

    void CreatePool()
    {
        // 총알을 생성해 밑으로 담아둘 빈 게임오브젝트 생성
        GameObject objectPools = new GameObject("ObjectPools");

        for(int i = 0; i < maxPool; ++i)
        {
            var obj = Instantiate(bulletPrefab, objectPools.transform);

            obj.name = "Bullet_" + i.ToString("00");

            obj.SetActive(false);

            bulletPool.Add(obj);
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; ++i)
        {
            if(bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

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
