using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자 클래스는 System.Serializable 이라는 Attribute를 명시해야 인스펙터 창에 노출됨
[System.Serializable]
public class PlayerAnim
{
	public AnimationClip idle;
	public AnimationClip runF;
	public AnimationClip runB;
	public AnimationClip runR;
	public AnimationClip runL;
}
public class PlayerCtrl : MonoBehaviour {

	private float horizontal = 0.0f;
	private float vertical = 0.0f;
	private float deltaAngle = 0.0f;

	private float moveSpeed = 10.0f;
	public float rotateSpeed = 100.0f;

	// 인스펙터 뷰에 표시할 애니메이션 클래스 변수
	public PlayerAnim playerAnim;

	// Animation 컴포넌트 인스턴스를 연결할 변수
	[HideInInspector]
	// = [System.NonSerialized]
	public Animation anim;

	//private Transform tr;
	//public Camera cam;

	private Vector3 movement = Vector3.zero;

	private void Awake()
	{
		// 스크립트가 실행될때 한번만 실행되는 함수
		// 주로 게임의 상태값 또는 변수 초기화에 사용됨
		// Start함수가 호출되기 이전에 호출
		// 스크립트가 비활성화 되어 있어도 호출
		
		// 코루틴으로 실행 할 수 없음.

	}
	// Use this for initialization
	void Start () {

		// Update 함수 최초 호출 이전에 한번만 호출
		// 스크립트가 활성화 되어 있어야 실행됨.
		// 모든 Awake 함수가 호출 되고 나서 실행.

		// 코루틴으로 실행 가능

		//tr = GetComponent<Transform>();
		//tr = GetComponent("Transform") as Transform;
		//tr = (Transform)GetComponent(typeof(Transform));

		// Animation 컴포넌트 인스턴스 연결
		anim = GetComponent<Animation>();

		// Animation 컴포넌트의 애니메이션 클립을 지정후 재생
		anim.clip = playerAnim.idle;
		anim.Play();

        moveSpeed = GameManager.instance.gameData.speed;
	}

	// Update is called once per frame
	void Update() {
		// 매 프레임마다 호출되는 함수로 주로 게임의 핵심 로직을 작성
		// 스크립트가 활성화 되어있어야 실행

		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
		deltaAngle = Input.GetAxis("Mouse X");

		// 이동 벡터를 vertical + horizontal로 구함
		movement =
			(Vector3.forward * vertical) + (Vector3.right * horizontal);

		// movement를 정규화해주고 계산
		movement = movement.normalized * moveSpeed * Time.deltaTime;

		// Transform.Translate(이동시킬만큼의 벡터 : Vector3);
		// 로컬 좌표계의 축을 기준으로 movement만큼 이동
		transform.Translate(movement, Space.Self);

		// Vector3.up 축을 기준으로 rotateSpeed 만큼의 속도로 회전
		transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime * deltaAngle);

		// 키보드 입력값을 기준으로 동작할 애니메이션 수행
		if (vertical >= 0.1f)
		{
			anim.CrossFade(playerAnim.runF.name, 0.3f);
		}
		else if (vertical <= -0.1f)
		{
			anim.CrossFade(playerAnim.runB.name, 0.3f);
		}
		else if (horizontal >= 0.1f)
		{
			anim.CrossFade(playerAnim.runR.name, 0.3f);
		}
		else if (horizontal <= -0.1f)
		{
			anim.CrossFade(playerAnim.runL.name, 0.3f);
		}
		else
		{
			anim.CrossFade(playerAnim.idle.name, 0.3f);
		}
	}

    void UpdateSetup()
    {
        moveSpeed = GameManager.instance.gameData.speed;
    }
	private void LateUpdate()
	{
		// 모든 Update 함수가 호출되고 나서 한번씩 호출
		// Update 함수에서 처리가 끝난후 실행해야 하는 로직에 사용
		// ex) 플레이어를 따라 움직이는 카메라 이동
		// 스크립트가 활성화 되어있어야 실행
		//cam.transform.Translate(movement, Space.Self);
	}

	private void FixedUpdate()
	{
		// 물리엔진의 시물레이션 계산 주기마다 호출되는 함수 (기본값 : 0.02초)
		// 발생하는 주기가 일정
	}

	private void OnEnable()
	{
        // 게임오브젝트 또는 스크립트가 활성화 되는 순간마다 실행
        // 이벤트 연결 같은 로직에 활용하기 용이
        // 코루틴 활용 불가

        GameManager.OnItemChange += UpdateSetup;
    }

	private void OnDisable()
	{
        // 게임오브젝트 또는 스크립트가 비활성화 되는 순간마다 실행
        // 이벤트 연결 종료 같은 로직에 활용하기 용이
        // 코루틴 활용 불가
        GameManager.OnItemChange -= UpdateSetup;
    }

    private void OnGUI()
	{
		// 개발용 테스트 UI 함수
	}
}
