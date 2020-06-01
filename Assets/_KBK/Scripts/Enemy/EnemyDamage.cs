using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    //생명게이지
    private float hp = 100f;
    // 피격시 사용할 혈흔 효과
    private GameObject bloodEffect;
    
    void Start()
    {
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == bulletTag)
        {
            showBloodEffect(collision);
            Destroy(collision.gameObject);
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            if(hp <= 0f)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
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
