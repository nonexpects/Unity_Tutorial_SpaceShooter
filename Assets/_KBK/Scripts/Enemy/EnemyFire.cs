using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    //AudioSource 컴포넌트 저장 변수
    private AudioSource audio;
    private Animator animator;
    private Transform playerTr;
    private Transform enemyTr;

    private readonly int hashFire = Animator.StringToHash("Fire");
    private readonly int hashReload = Animator.StringToHash("Reload");

    private float nextFire = 0f;
    private readonly float fireRate = 0.1f;
    private readonly float damping = 10f;

    //재장전시간
    private readonly float reloadTime = 2f;
    private readonly int maxBullet = 10;
    private int currBullet = 10;
    private bool isReload = false;
    //재장전 시간동안 기다릴 변수 선언
    private WaitForSeconds wsReload;

    public bool isFire = false;
    public AudioClip fireSfx;
    public AudioClip reloadSfx;

    //적 캐릭터 총알 프리팹
    public GameObject Bullet;
    //총알 발사 위치 정보
    public Transform firePos;
    //MuzzleFlash의 MeshRenderer 컴포넌트 저장할 변수
    public MeshRenderer muzzleFlash;
    
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();

        wsReload = new WaitForSeconds(reloadTime);
    }
    
    void Update()
    {
        if(!isReload && isFire)
        {
            if(Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate + Random.Range(0f, 0.3f);
            }

            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    void Fire()
    {
        animator.SetTrigger(hashFire);
        audio.PlayOneShot(fireSfx, 1f);
        StartCoroutine(ShowMuzzleFlash());

        GameObject _bullet = Instantiate(Bullet, firePos.position, firePos.rotation);
        Destroy(_bullet, 3f);

        //남은 총알로 재장전 여부 계산
        isReload = (--currBullet % maxBullet == 0);
        if(isReload)
        {
            StartCoroutine(Reloading());
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        //MuzzleFlash 활성화
        muzzleFlash.enabled = true;

        //불규칙한 회전 각도 계산
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        //MuzzleFlash Z축 방향으로 회전
        muzzleFlash.transform.localRotation = rot;
        //MuzzleFlash의 스케일을 불규칙하게 조정
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1f, 2f);

        //텍스처의 offset 속성에 적용할 불규칙한 값 생성
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        //MuzzleFlash의 머터리얼의 Offset 값을 적용
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);

        //MuzzleFlash가 보일 동안 잠시 대기 
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        //MuzzleFlash 다시 비활성화
        muzzleFlash.enabled = false;
    }

    IEnumerator Reloading()
    {
        //MuzzleFlash 다시 비활성화
        muzzleFlash.enabled = false;
        animator.SetTrigger(hashReload);
        audio.PlayOneShot(reloadSfx, 1f);

        yield return wsReload;

        //총알 개수 초기화
        currBullet = maxBullet;
        isReload = false;
    }
}
