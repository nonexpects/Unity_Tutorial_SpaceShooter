using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//총알 발사와 재장전 오디오 클립을 저장할 구조체
[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    // 무기 타입
    public enum WeaponType
    {
        RIFLE = 0,
        SHOTGUN
    }

    // 주인공이 현재 들고 있는 무기를 저장할 변수
    public WeaponType currWeapon = WeaponType.RIFLE;

    public GameObject bullet;
    public Transform firePos;
    // 탄피 추출 파티클
    public ParticleSystem cartridge;
    // 총구 화염 파티클
    public ParticleSystem muzzleFlash;

    // AudioSource 컴포넌트를 저장할 변수
    private AudioSource _audio;
    // 오디오 클립을 저장할 변수
    public PlayerSfx playerSfx;

    //Shake 클래스 저장할 변수
    private Shake shake;

    //탄창 이미지 UI
    public Image magazineImg;
    //남은 총알 수 Text UI
    public Text magazineText;

    //최대 총알 수
    public int maxBullet = 10;
    //남은 총알 수
    public int remainingBullet = 10;

    //재장전 시간
    public float reloadTime = 2f;
    //재장전 여부 판단할 변수
    public bool isReloading = false;

    //변경할 무기 이미지
    public Sprite[] weaponIcons;
    //교체할 무기 이미지 UI
    public Image weaponImage;

    //적캐릭터 레이어값 저장 변수
    private int enemyLayer;
    //장애물의 레이어 값 저장할 변수
    private int obstacleLayer;
    //레이어 마스크의 비트연산 위한 변수
    private int layerMask;

    //자동 발사 여부 판단 변수
    private bool isFire = false;
    //다음 발사 시간 저장 변수
    private float nextFire;
    //총알 발사 간격
    public float fireRate = 0.1f;

    void Start()
    {
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        // AudioSource 컴포넌트 추출
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        //적 캐릭터 레이어 값 추출
        enemyLayer = LayerMask.NameToLayer("ENEMY");
        //장애물 레이어 값 추출
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        //레이어 마스크의 비트연산(OR연산)
        layerMask = 1 << obstacleLayer | 1 << enemyLayer;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(firePos.position, firePos.forward * 20f, Color.green);

        if (EventSystem.current.IsPointerOverGameObject()) return;

        //레이캐스트에 검출된 객체의 정보 저장 변수
        RaycastHit hit;

        //레이캐스트 생성해 적 캐릭터 검출
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, 1 << enemyLayer))
        {
            isFire = (hit.collider.CompareTag("ENEMY"));
        }
        else
            isFire = false;

        //레이캐스트에 적 캐릭터 닿았을 때 자동 발사
        if(!isReloading && isFire)
        {
            if(Time.time > nextFire)
            {
                //총알 수 하나 감소
                --remainingBullet;
                Fire();

                //남은 총알 없는 경우 재장전 코루틴 호출
                if(remainingBullet == 0)
                {
                    StartCoroutine(Reloading());
                }
                //다음 총알 발사시간 계산
                nextFire = Time.time + fireRate;
            }
        }
        if(!isReloading && Input.GetMouseButtonDown(0))
        {
            --remainingBullet;
            Fire();

            if(remainingBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }
    }

    void Fire()
    {
        //셰이크 효과 연출
        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f));
        //Instantiate(bullet, firePos.position, firePos.rotation);

        var _bullet = GameManager.instance.GetBullet();
        if(_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }
        cartridge.Play();
        muzzleFlash.Play();
        // 사운드 발생
        FireSfx();

        //재장전 이미지의 fillAmount 속성값 지정
        magazineImg.fillAmount = (float)remainingBullet / (float)maxBullet;
        //남은 총알 수 갱신
        UpdateBulletText();
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1f);

        //재장전 오디오 길이 + 0.3초동안 대기
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        //각종 변숫값의 초기화
        isReloading = false;
        magazineImg.fillAmount = 1f;
        remainingBullet = maxBullet;
        //남은 총알 수 갱신
        UpdateBulletText();
    }

    private void UpdateBulletText()
    {
        magazineText.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainingBullet, maxBullet);
    }

    void FireSfx()
    {
        // 현재 들고 있는 무기의 오디오 클립을 가져옴
        var _sfx = playerSfx.fire[(int)currWeapon];
        // 사운드 발생
        _audio.PlayOneShot(_sfx, 1f);
    }

    public void OnChangeWeapon()
    {
        currWeapon = (WeaponType)((int)++currWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currWeapon];
    }
}
