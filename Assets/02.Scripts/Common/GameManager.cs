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

    // 인벤토리의 아이템이 변경됐을때 발생 시킬 이벤트 정의
    public delegate void ItemChangeDelegate();
    public static event ItemChangeDelegate OnItemChange;

    private GameObject slotList;
    public GameObject[] itemObjects;

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

            slotList = inventoryCG.transform.Find("SlotList").gameObject;

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
        gameData.equipedItems = data.equipedItems;

        // 보유한 아이템이 있다면 인벤토리 셋팅
        if (gameData.equipedItems.Count > 0)
        {
            InventorySetup();
        }
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

    // 로드한 데이터를 기준으로 인벤토리에 아이템을 추가
    void InventorySetup()
    {
        // SlotList 하위에 있는 모든 Slot을 추출
        var slots = slotList.GetComponentsInChildren<Transform>();

        // 보유한 아이템의 개수만큼 반복
        for (int i = 0; i < gameData.equipedItems.Count; ++i)
        {
            for (int j = 1; j < slots.Length; j++)
            {
                //Slot 하위에 다른 아이템이 있으면 다음 인덱스로 스킵
                if (slots[j].childCount > 0) continue;

                // 보유한 아이템의 종류에 따라 인덱스를 추출
                int itemIndex = (int)gameData.equipedItems[i].itemType;

                // 아이템의 부모를 Slot 게임오브젝트로 변경
                itemObjects[itemIndex].GetComponent<Transform>().SetParent(slots[j]);

                // 아이템의 ItemInfo 클래스의 itemData에 로드했던 데이터를 저장함
                itemObjects[itemIndex].GetComponent<ItemInfo>().itemData = gameData.equipedItems[i];

                break;
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
    #endregion

    // 인벤토리에 아이템을 추가했을때 데이터의 정보를 갱신하는 함수
    public void AddItem(Item item)
    {
        if (gameData.equipedItems.Contains(item)) return;

        gameData.equipedItems.Add(item);

        switch (item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.hp += item.value;
                else
                    gameData.hp = gameData.hp * (1.0f + item.value);
                break;
            case Item.ItemType.DAMAGEUP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.damage += item.value;
                else
                    gameData.damage = gameData.damage * (1.0f + item.value);
                break;
            case Item.ItemType.SPEEDUP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.speed += item.value;
                else
                    gameData.speed = gameData.speed * (1.0f + item.value);
                break;
            case Item.ItemType.GRENADE:
                break;
        }
        if (OnItemChange != null) OnItemChange();
    }

    public void RemoveItem(Item item)
    {
        gameData.equipedItems.Remove(item);

        if (gameData.equipedItems.Contains(item)) return;

        switch (item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.hp -= item.value;
                else
                    gameData.hp = gameData.hp / (1.0f + item.value);
                break;
            case Item.ItemType.DAMAGEUP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.damage -= item.value;
                else
                    gameData.damage = gameData.damage / (1.0f + item.value);
                break;
            case Item.ItemType.SPEEDUP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.speed -= item.value;
                else
                    gameData.speed = gameData.speed / (1.0f + item.value);
                break;
            case Item.ItemType.GRENADE:
                break;
        }
        if (OnItemChange != null) OnItemChange();
    }
}
