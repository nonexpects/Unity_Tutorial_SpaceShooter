using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //오브젝트 풀링 생성함수 호출
        CreatePooing();
    }

    void Start()
    {
        //하이러키 뷰의 SpawnPointGroup을 찾아 하위에 있는 모든 Transform 컴포넌트 찾아옴
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        if(points.Length > 0)
        {
            StartCoroutine(this.CreateEnemy());
        }
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
        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공 캐릭터에 추가된 모든 스크립트 추출
        var scripts = playerObj.GetComponents<MonoBehaviour>();
        //주인공 캐릭터의 모든 스크립트 활성화/비활성화
        foreach(var script in scripts)
        {
            script.enabled = !isPaused;
        }
    }

}
