using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    //적 캐릭터 추적 사정 거리 범위
    public float viewRange = 15f;
    [Range(0, 360)]
    //적 캐릭터 시야각
    public float viewAngle = 120f;

    private Transform enemyTr;
    private Transform playerTr;
    private int playerLayer;
    private int obstacleLayer;
    private int layerMask;
    void Start()
    {
        //컴포넌트 추출
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").transform;

        //레이어 마스크값 계산
        playerLayer = LayerMask.NameToLayer("PLAYER");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        layerMask = 1 << playerLayer | 1 << obstacleLayer;

    }
    
    //주어진 각도에 의해 원주 위의 점의 좌푯값 계산하는 함수
    public Vector3 CirclePoint(float angle)
    {
        //로컬 좌표계 기준으로 설정하기 위해 적 캐릭터 Y회전값 더함
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool isTracePlayer()
    {
        bool isTrace = false;

        //추적반경 범위 안에서 주인공 캐릭터 추출
        Collider[] colls = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);
        //배열 개수 1일때 주인공이 범위 안에 있다고 판다
        if(colls.Length == 1)
        {
            //적캐릭터와 주인공 사이 방향 벡터 계산
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;

            //적캐릭터 시야각에 들어왔는지 판단
            if(Vector3.Angle(enemyTr.forward, dir)< viewAngle * 0.5f)
            {
                isTrace = true;
            }

           
        }
        return isTrace;
    }

    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;

        //적캐릭터와 주인공 사이 방향 벡터 계산
        Vector3 dir = (playerTr.position - enemyTr.position).normalized;

        //적캐릭터 시야각에 들어왔는지 판단
        if (Physics.Raycast(enemyTr.position, dir, out hit, viewRange, layerMask))
        {
            isView = (hit.collider.CompareTag("PLAYER"));
        }
        return isView;

    }
}

