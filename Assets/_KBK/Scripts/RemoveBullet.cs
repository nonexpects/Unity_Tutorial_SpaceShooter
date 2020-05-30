using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    // 스파크 프리팹을 저장할 변수 
    public GameObject sparkEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "BULLET")
        {
            // 스파크 효과 함수 호출
            ShowEffect(collision);
            Destroy(collision.gameObject);
        }
    }

    void ShowEffect(Collision collision)
    {
        // 충돌 지점 정보를 추출
        ContactPoint contact = collision.contacts[0];
        // 법선 벡터가 이루는 회전 각도를 추출
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        // 스파크 효과를 생성
        GameObject spark = Instantiate(sparkEffect, contact.point+ (-contact.normal * 0.05f), rot);

        // 스파크 효과의 부모를 드럼통 또는 벽으로 설정
        spark.transform.SetParent(this.transform);
    }
}
