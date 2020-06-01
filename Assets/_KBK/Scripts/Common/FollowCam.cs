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

    //CameraRig의 transform 컴포넌트
    private Transform tr;

    void Start()
    {
        tr = GetComponent<Transform>();
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
    }
}
