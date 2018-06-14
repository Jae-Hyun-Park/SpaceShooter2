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
	// Use this for initialization
	void Start () {

        // agent 연결
        agent = GetComponent<NavMeshAgent>();

        agent.autoBraking = false;

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

        MoveWayPoint();
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

	// Update is called once per frame
	void Update () {
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
