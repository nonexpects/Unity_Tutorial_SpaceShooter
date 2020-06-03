using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//GameData, Item 클래스가 담긴 네임스페이스 명시
using DataInfo;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Create Info")]
    //적 캐릭터가 출현할 위치 담을 배열
    public Transform[] points;
    //적 캐릭터 프리팹 저장 변수
    public GameObject enemy;
    //적 캐릭터 생성 주기
    public float createTime = 2f;
    //적 캐릭터 최대 생성 개수
    public int maxEnemy = 10;
    //게임 종류 여부 판단 변수
    public bool isGameOver = false;

    //싱글턴에 접근하기 위한 Static 변수 선언
    public static GameManager instance = null;

    private bool isPaused;
    //Inventory의 CanvasGroup 컴포넌트를 저장할 변수
    public CanvasGroup inventoryCG;

    //주인공이 죽인 적 캐릭터 수
    //[HideInInspector] public int killCount;
    [Header("GameData")]
    //적 캐릭터를 죽인 횟수를 표시할 텍스트 UI
    public Text killCountTxt;
    //DataManager 저장할 변수
    private DataManager dataManger;
    public GameData gameData;

    [Header("Object Pool")]
    //생성할 총알 프리팹
    public GameObject bulletPrefab;
    //오브젝트 풀에 생성할 개수
    public int maxPool = 10;
    public List<GameObject> bulletPool = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        //instance에 할당 된 클래스의 인스턴스가 다를 경우 새로 생성된 클래스를 의미
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        //DataManager 추출해 저장
        dataManger = GetComponent<DataManager>();
        //DataManager 초기화
        dataManger.Initialize();

        //게임 초기 데이터 로드
        LoadGameData();

        //오브젝트 풀링 생성함수 호출
        CreatePooing();
    }

    //게임 초기 데이터 로드
    private void LoadGameData()
    {
        //KILL_COUNT 키로 저장된 값 로드
        //killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        killCountTxt.text = "KILL " + gameData.killCount.ToString("0000");

        //DataManager를 통해 파일에 저장된 데이터 불러오기
        GameData data = dataManger.Load();
        gameData.killCount = data.killCount;
        gameData.hp = data.hp;
        gameData.speed = data.speed;
        gameData.damage = data.damage;
        gameData.equipItem = data.equipItem;
    }

    void SaveGameData()
    {
        dataManger.Save(gameData);
    }

    void Start()
    {
        //처음 인벤토리 비활성화
        OnInventoryOpen(false);
        //하이러키 뷰의 SpawnPointGroup을 찾아 하위에 있는 모든 Transform 컴포넌트 찾아옴
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        if(points.Length > 0)
        {
            StartCoroutine(this.CreateEnemy());
        }
    }

    //인벤토리 활성화/비활성화하는 함수
    public void OnInventoryOpen(bool isOpened)
    {
        inventoryCG.alpha = (isOpened) ? 1f : 0f;
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts = isOpened;
    }

    IEnumerator CreateEnemy()
    {
        while(!isGameOver)
        {
            int enemyCount = (int)GameObject.FindGameObjectsWithTag("ENEMY").Length;

            if(enemyCount < maxEnemy)
            {
                yield return new WaitForSeconds(createTime);

                //불규칙적인 위치 산출
                int idx = Random.Range(1, points.Length);
                //적 캐릭터 동적 생성
                Instantiate(enemy, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    private void CreatePooing()
    {
        //총알 생성해 차일드화할 페어런트 게임오브젝트 생성
        GameObject objectPools = new GameObject("ObjectPools");

        //풀링 개수만큼 미리 총알 생ㅇ성
        for (int i = 0; i < maxPool; i++)
        {
            var obj = Instantiate<GameObject>(bulletPrefab, objectPools.transform);
            obj.name = "Bullet_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 생성한 총알 추가 
            bulletPool.Add(obj);
        }
    }

    public void OnPauseClick()
    {
        //일시정지값 토글
        isPaused = !isPaused;
        //Time Scale이 0이면 정지, 1이면 정상속도
        Time.timeScale = (isPaused) ? 0f : 1f;
        //주인공 객체 추출
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        //주인공 캐릭터에 추가된 모든 스크립트 추출
        var scripts = playerObj.GetComponents<MonoBehaviour>();
        //주인공 캐릭터의 모든 스크립트 활성화/비활성화
        foreach(var script in scripts)
        {
            script.enabled = !isPaused;
        }

        var canvasGroup = GameObject.Find("Panel - Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }

    //적 캐릭터가 죽을 때마다 호출될 함수
    public void IncKillCount()
    {
        ++gameData.killCount;
        killCountTxt.text = "KILL " + gameData.killCount.ToString("0000");
        //++killCount;
        //killCountTxt.text = "KILL " + killCount.ToString("0000");
        //죽인 횟수 저장
        //PlayerPrefs.SetInt("KILL_COUNT", killCount);
    }

    private void OnApplicationQuit()
    {
        //게임 종료 전 게임 데이터 제장
        SaveGameData();
    }

}
