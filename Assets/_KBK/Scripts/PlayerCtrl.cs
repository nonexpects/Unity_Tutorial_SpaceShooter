using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
}

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0f;
    private float v = 0f;
    private float r = 0f;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    private Transform tr;

    // 이동 속도 변수(public으로 선언되어 Inspector에 노출됨)
    public float moveSpeed = 10f;
    public float rotSpeed = 80f;

    // 인스펙터 뷰에 표시할 애니메이션 클래스 변수
    public PlayerAnim playerAnim;
    // Animation 컴포넌트를 저장하기 위한 변수
    [HideInInspector] // or [System.Nonserialized] 사용
    public Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        // 스크립트가 실행된 후 처음 수행되는 Start 함수에서 Transform 컴포넌트를 할당
        tr = GetComponent<Transform>();

        // Animation 컴포넌트를 변수에 할당
        anim = GetComponent<Animation>();
        anim.clip = playerAnim.idle;
        anim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        //Debug.Log("h = " + h.ToString());
        //Debug.Log("v = " + v.ToString());

        // 전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        
        // Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준좌표(월드,로컬))
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        // Vector3.up 축을 기준으로 rotSpeed 만큼의 속도로 회전
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);

        if(v >= 0.1f)
        {
            anim.CrossFade(playerAnim.runF.name, 0.3f); // fadeLength 앞으로 이동
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f); // 뒤로 이동
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f); // 오른쪽 이동
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f); // 왼쪽 이동 
        }
        else
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f); // 정지시 Idle 
        }
    }
}
