using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    // 적의 상태 정의
    public enum EnemyState
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE,
    }

    // 상태를 저장 할 변수 : 초기값 PATROL
    public EnemyState state = EnemyState.PATROL;

    // 플레이어의 Transform 
    public Transform playerTransform;

    // 추적 사정거리
    public float traceDistance = 10.0f;

    // 공격 사정거리
    public float attackDistance = 5.0f;

    // AI 상태 체크 주기
    public float refreshAICycle = 0.3f;

    // 죽어있는 상태
    bool isDie = false;

    // 코루틴에서 사용할 지연 시간 변수
    private WaitForSeconds waitSecond;

    // MoveAgent의 인스턴스 저장 변수
    private MoveAgent moveAgent;

    // Animator 컴포넌트 변수
    private Animator animator;

    private EnemyFire enemyFire;

    // 애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIndex = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    private void Awake()
    {
        // Player 오브젝트를 씬에서 찾음
        var playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            playerTransform = playerObject.GetComponent<Transform>();

        moveAgent = GetComponent<MoveAgent>();
        animator = GetComponent<Animator>();
        enemyFire = GetComponent<EnemyFire>();

        waitSecond = new WaitForSeconds(refreshAICycle);
    }

    // 활성화 되는 순간 호출
    private void OnEnable()
    {
        // CycleOffset 랜덤 설정
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));

        // WalkSpeed 값을 랜덤 설정
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 1.2f));
        // CheckState (코루틴) 호출
        StartCoroutine(CheckState());

        // Action 코루틴 호출
        StartCoroutine(Action());

        Damage.onPlayerDie += OnPlayerDie;
    }

    private void OnDisable()
    {
        Damage.onPlayerDie -= OnPlayerDie;
    }

    IEnumerator CheckState()
    {
        while (!isDie)
        {
            if(state == EnemyState.DIE)
                yield break;

            // 주인공과 적 캐릭터의 사이의 거리를 계산
            //float dist = Vector3.Distance(playerTransform.position, transform.position);            // 실수연산을 하므로 CPU에 부하가 심하다 따라서 sqrMagnitude를 이용하여 제곱수를 사용한다.
            float dist = (playerTransform.position - transform.position).sqrMagnitude;

            if (dist <= attackDistance * attackDistance)
            {
                state = EnemyState.ATTACK;
            }
            else if (dist <= traceDistance * traceDistance)
            {
                state = EnemyState.TRACE;
            }
            else
            {
                state = EnemyState.PATROL;
            }
            yield return waitSecond;
        }
    }
    // Use this for initialization
    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return waitSecond;

            switch (state)
            {
                case EnemyState.PATROL:
                    // 순찰 모드 활성화
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case EnemyState.TRACE:
                    // 주인공의 위치를 넘겨 추적모드로 변경
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTransform.position;
                    animator.SetBool(hashMove, true);
                    break;
                case EnemyState.ATTACK:
                    if (enemyFire.isFire == false) {  enemyFire.isFire = true; }
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    break;
                case EnemyState.DIE:
                    isDie = true;
                    enemyFire.isFire = false;

                    // 순찰 및 추적 정지
                    moveAgent.Stop();

                    // 사망 애니메이션 종류를 지정
                    animator.SetInteger(hashDieIndex, Random.Range(0, 3));

                    // 사망 트리거 전달
                    animator.SetTrigger(hashDie);
                    break;
            }
        }
    }
    void Start () {
		
	}
	// Update is called once per frame
	void Update () {
        // Speed 파라미터에 이동속도를 전달
        animator.SetFloat(hashSpeed, moveAgent.speed);
	}
    
    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;

        // 모든 코루틴 함수 종료
        StopAllCoroutines();

        animator.SetTrigger(hashPlayerDie);
    }
}
