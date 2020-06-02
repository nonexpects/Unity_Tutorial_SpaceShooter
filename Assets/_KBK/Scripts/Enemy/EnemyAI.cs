using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.PATROL;

    private Transform playerTr;
    private Transform enemyTr;
    private Animator animator;

    public float attackDist = 5f;
    public float traceDist = 10f;

    public bool isDie = false;

    //코루틴에서 사용할 지연시간 변수
    private WaitForSeconds ws;
    //이동 제어하는 MoveAgent 클래스 저장할 변수
    private MoveAgent moveAgent;
    private EnemyFire enemyFire;

    //애니메이션 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        
        if(player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }

        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        moveAgent = GetComponent<MoveAgent>();
        enemyFire = GetComponent<EnemyFire>();

        //코루틴 지연시간 생성
        ws = new WaitForSeconds(0.3f);
        //Cycle Offset 값 불규칙하게 변경
        animator.SetFloat(hashOffset, Random.Range(0f, 1f));
        //Speed값 불규칙하게 변경
        animator.SetFloat(hashWalkSpeed, Random.Range(1f, 1.2f));
    }

    private void OnEnable()
    {
        //CheckState 코루틴 함수 실행
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        Damage.OnPlayerDie += this.OnPlayerDie;
    }

    private void OnDisable()
    {
        Damage.OnPlayerDie -= this.OnPlayerDie; 
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        StopAllCoroutines();

        animator.SetTrigger(hashPlayerDie);
    }

    IEnumerator Action()
    {
        while(!isDie)
        {
            yield return ws;
            switch (state)
            {
                case State.PATROL:
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);

                    if (enemyFire.isFire == false)
                        enemyFire.isFire = true;
                    break;
                case State.DIE:
                    isDie = true;
                    enemyFire.isFire = false;
                    moveAgent.Stop();
                    animator.SetInteger(hashDieIdx, Random.Range(0, 3));
                    animator.SetTrigger(hashDie);
                    //사망후에 혈흔 안남도록 Capsule Collider 컴포넌트 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    IEnumerator CheckState()
    {
        while(!isDie)
        {
            if (state == State.DIE) yield break;

            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            if (dist <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (dist <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
            yield return ws;
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }
}
