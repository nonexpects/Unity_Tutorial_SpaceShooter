using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float moveDamping = 15f; // 이속 계수
    public float rotateDamping = 10f; // 회전 속도 계수 계수
    public float distance = 5f; // 추적 대상과의 거리
    public float height = 4f; // 추적 대상과의 높이
    public float targetOffset = 2f; // 추적 좌표 오프셋

    [Header("Wall Obstacles Setting")]

    public float heightAboveWall = 7f;
    public float colliderRadius = 1f;
    public float overDamping = 5f;//이동속도 계수
    private float originHeight; //최초높이 보관 변수

    [Header("Etc Obstacle Setting")]
    //카메라가 올라갈 높이
    public float heighAboveObtsacle = 12f;
    //주인공에 투사할 레이캐스트의 높이 오프셋
    public float castOffset = 1f;

    //CameraRig의 transform 컴포넌트
    private Transform tr;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    private void Update()
    {
        //구체 형태의 충돌체로 충돌여부 검사
        if(Physics.CheckSphere(tr.position, colliderRadius))
        {
            //보간 이용해서 카메라의 높이를 부드럽게 상승
            height = Mathf.Lerp(height, heightAboveWall, Time.deltaTime * overDamping);
        }
        else
        {
            //보간 이용해서 카메라의 높이를 부드럽게 하강
            height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
        }

        //주인공이 장애물에 가려졌는지를 판단할 레이캐스트의 높낮이
        Vector3 castTarget = target.position + (target.up * castOffset);
        //cast Target좌표로의 방향 벡터 계산
        Vector3 castDir = (castTarget - tr.position).normalized;
        //충돌 정보 반환받을 변수
        RaycastHit hit;

        //레이캐스트 투사해 장애물 여부 검사
        if(Physics.Raycast(tr.position, castDir, out hit , Mathf.Infinity))
        {
            //주인공을 레이캐스트에 맞지 않았을 경우
            if(!hit.collider.CompareTag("PLAYER"))
            {
                //보간 이용해서 카메라의 높이를 부드럽게 상승
                height = Mathf.Lerp(height, heightAboveWall, Time.deltaTime * overDamping);
            }
            else
            {
                //보간 이용해서 카메라의 높이를 부드럽게 하강
                height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
            }
        }
    }

    private void LateUpdate()
    {
        // 카메라의 높이와 거리 계산
        var camPos = target.position - (target.forward * distance) + (target.up * height);

        //이동 할 때의 속도 계수 적용
        tr.position = Vector3.Slerp(tr.position, camPos, Time.deltaTime * moveDamping);
        //회전 할 때의 속도 계수 적용
        tr.rotation = Quaternion.Slerp(tr.rotation, target.rotation, Time.deltaTime * rotateDamping);

        //카메라 추적대상으로 Z축 회전시킴
        tr.LookAt(target.position + (target.up * targetOffset));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //추적 및 시야 맞출 위치 표시
        Gizmos.DrawWireSphere(target.position + (target.up * targetOffset), 0.1f);
        //메인 카메라와 추적 지점 간의 선 표시
        Gizmos.DrawLine(target.position + (target.up * targetOffset), transform.position);

        //카메라의 충돌체 표현하기 위한 구체 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);

        //주인공 캐릭터가 장애물에 가려졌는지 판단할 레이 표시
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target.position + (target.up * castOffset), transform.position);
    }
}
