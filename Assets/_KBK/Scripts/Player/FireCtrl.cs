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

    void Start()
    {
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        // AudioSource 컴포넌트 추출
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
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
