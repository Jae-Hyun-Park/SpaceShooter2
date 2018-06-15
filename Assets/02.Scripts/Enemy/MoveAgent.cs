using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour {

    // 순찰지점들을 저장할 List 변수
    public List<Transform> wayPoints;

    // 다음 순찰 지점의 인덱스
    public int nextIndex = 0;

    // NavMeshAgent 컴포넌트 인스턴스
    private NavMeshAgent agent;

    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;

    // 회전 속도를 조절하는 계수
    private float damping = 1.0f;

    // Patrol 중인지 판단하는 변수
    private bool _patrolling;

    // patrolling 프로퍼티 정의 (getter, setter)
    public bool patrolling
    {
        get {
            return _patrolling;
        }
        set {
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                damping = 1.0f;
                MoveWayPoint();
            }
        }
    }

    // 추적 대상의 위치를 제어하기 위한 변수
    private Vector3 _traceTarget;
    // traceTarget 프로퍼티 정의 (getter, setter)
    public Vector3 traceTarget
    {
        get {  return _traceTarget;  }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }

    // NavMeshAgent의 이동속도 프로퍼티
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }
    
    // Use this for initialization
    void Start () {

        // agent 연결
        agent = GetComponent<NavMeshAgent>();

        agent.autoBraking = false;

        // 자동으로 회전하는 기능을 꺼줌
        agent.updateRotation = false;

        agent.speed = patrolSpeed;

        var group = GameObject.Find("WayPointGroup");
        
        if (group != null)
        {
            // WayPointGroup 하위에 있는 모든 Transform 컴포넌트를
            // List타입의 WayPoints에 추가

            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);

            //foreach(Transform child in group.transform)
            //{
            //    wayPoints.Add(child);
            //}
        }

        //MoveWayPoint();
        patrolling = true;
    }
	
    // 다음 목적지 까지 이동 명령을 내리는 함수
    void MoveWayPoint()
    {
        // 최단거리 경로 계산이 끝나지 않았으면 함수 종료
        if (agent.isPathStale) return;

        // 다음 목적지를 wayPoints 배열에서 nextIndex로 가져와 지정한다.
        agent.destination = wayPoints[nextIndex].position;

        // 내비게이션 기능을 활성화
        agent.isStopped = false;
    }

    // Player를 추적할 때 이동시키는 함수
    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;

        agent.destination = pos;
        agent.isStopped = false;
    }

    public void Stop()
    {
        agent.isStopped = true;

        // 바로 정지하기 위한 velocity를 0로 초기화

        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
	// Update is called once per frame
	void Update () {

        // 적 캐릭터가 이동중일 때만 회전
        if (agent.isStopped == false)
        {
            // NavMeshAgent가 회전할 쿼터니언 타입의 회전 정보를 바라보아야 할 방향 벡터에서부터 계산
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * damping);
        }
        if (!_patrolling) return;

		// NavMeshAgent가 이동하고 있고 목적지에 도착했는지 계산
        if (agent.velocity.sqrMagnitude > 0.2f * 0.2f && agent.remainingDistance <= 0.5f)   // 도착했다면
        {
            // 다음 목적지의 배열 인덱스를 계산
            nextIndex = ++nextIndex % wayPoints.Count;

            // 다음 목적지로 이동명령을 수행
            MoveWayPoint();
        }
	}
}
