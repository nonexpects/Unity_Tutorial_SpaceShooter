using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    //생명게이지
    private float hp = 100f;
    private float initHp = 100f;
    // 피격시 사용할 혈흔 효과
    private GameObject bloodEffect;

    //생명 게이지 프리팹 저장 변수
    public GameObject hpBarPrefab;
    //생명 게이지 위치 보정할 오프셋
    public Vector3 hpBarOffset = new Vector3(0, 2.2f, 0);
    //부모가 될 Canvas 객체
    private Canvas uiCanvas;
    //생명수치에 따라 fillAmount 속성 변경할 Image
    private Image hpBarImage;
    
    void Start()
    {
        //enemyAI = GetComponent<EnemyAI>();

        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");

        //생명게이지 생성 및 초기화
        SetHpBar();
    }

    private void SetHpBar()
    {
        uiCanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
        //Ui Canvas 하위로 생명게이지 생성
        GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform);
        //fillAmount속성 변경할 Image 추출
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        //생명게이지가 따라가야 할 대상과 오프셋 값 설정
        var _hpBar = hpBar.GetComponent<EnemyHpBar>();
        _hpBar.targetTr = this.gameObject.transform;
        _hpBar.offset = hpBarOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == bulletTag)
        {
            showBloodEffect(collision);
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            hpBarImage.fillAmount = hp / initHp;
            //hp--;
            if (hp <= 0f)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;
            }
        }
    }

    private void showBloodEffect(Collision collision)
    {
        //총알 충돌 지점
        Vector3 pos = collision.contacts[0].point;
        //총알 충돌했을 떄 법선 벡터
        Vector3 _normal = collision.contacts[0].normal;
        //총알 충돌시 방향 벡터 회전값 계산
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        //혈흔 효과 생성
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1f);
    }
}
