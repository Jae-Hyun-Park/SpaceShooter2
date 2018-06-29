using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataInfo;
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

    private bool isPaused = false;

    // 인벤토리의 CanvasGroup 컴포넌트 변수
    public CanvasGroup inventoryCG;

    //[HideInInspector] public int killCount;
    #region GameData
    public Text killCountText;
    private DataManager dataManager;
    public GameData gameData;
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

            dataManager = GetComponent<DataManager>();
            dataManager.Initialized();

            LoadGameData();
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
        OnInventoryButtonClick(false);

        spawnPoints = GameObject.Find("EnemySpawnPointGroup").GetComponentsInChildren<Transform>();
        LoadGameData();

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

    public void OnPauseClick()
    {
        isPaused = !isPaused;

        Time.timeScale = (isPaused) ? 0.0f : 1.0f;

        var PlayerObject = GameObject.FindGameObjectWithTag("Player");

        var components = PlayerObject.GetComponents<MonoBehaviour>();

        foreach(var component in components)
        {
            component.enabled = !isPaused;
        }

        var canvasGroup = GameObject.Find("Panel - Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }

    public void OnInventoryButtonClick(bool open)
    {
        inventoryCG.alpha = (open) ? 1.0f : 0.0f;
        inventoryCG.interactable = open;
        inventoryCG.blocksRaycasts = open;
    }

    #region 게임 데이터 저장 및 로드
    void LoadGameData()
    {
        GameData data = dataManager.Load();

        gameData.killCount = data.killCount;
        gameData.hp = data.hp;
        gameData.speed = data.speed;
        gameData.damage = data.damage;
        gameData.equipedItem = data.equipedItem;

        //killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        killCountText.text = "KILL : " + gameData.killCount.ToString("0000");
    }

    public void IncreaseKillCount()
    {
        ++gameData.killCount;
        killCountText.text = "KILL : " + gameData.killCount.ToString("0000");

        //PlayerPrefs.SetInt("KILL_COUNT", killCount);
    }

    void SaveGameData()
    {
        dataManager.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
    #endregion
}
