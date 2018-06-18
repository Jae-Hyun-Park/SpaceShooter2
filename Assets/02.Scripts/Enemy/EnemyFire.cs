using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour {
    
    // AudioSource 컴포넌트를 저장할 변수
    private AudioSource audioSource;

    // Animator 컴포넌트를 저장할 변수
    private Animator animator;

    // Player의 Transform
    private Transform playerTransform;

    // 애니메이터에 정의한 파라미터 Fire의 해시값을 추출
    private readonly int hashFire = Animator.StringToHash("Fire");
    private readonly int hashReload = Animator.StringToHash("Reload");
    // 총알 발사 여부 
    public bool isFire = false;

    // 총알 발사 간격
    public float fireRate = 0.5f;

    private float nextFire = 0.0f;

    // 회전 속도를 조절하는 계수
    private float damping = 10.0f;

    private readonly float reloadTime = 2.0f;   // 재장전 하는데 걸리는 시간
    private readonly int maxBullet = 10;        // 최대 탄창 갯수
    private int currentBullet = 10;             // 현재 남은 총알
    private bool isReload = false;              // 재장전 애니메이션 여부

    private WaitForSeconds wsReload;            // 재장전 시간동안 기다릴 WaitForSeconds 변수

    // 적 캐릭터의 총알 프리팹
    public GameObject bullet;

    // 총알 발사 위치 정보
    public Transform firePos;

    // 재장전 사운드 클립
    public AudioClip reloadSFX;

    // 총알 발사 사운드 클립
    public AudioClip FireSFX;

	// Use this for initialization
	void Start () {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        wsReload = new WaitForSeconds(reloadTime);
	}
	
	// Update is called once per frame
	void Update () {
        if (isFire && !isReload)
        {
            if(Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate;
            }

            // 주인공이 있는 위치까지의 회전 각도를 계산
            Quaternion rotation = Quaternion.LookRotation(playerTransform.position - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
	}

    void Fire()
    {
        animator.SetTrigger(hashFire);
        audioSource.PlayOneShot(FireSFX);

        // 총알 생성
        GameObject _bullet = Instantiate(bullet, firePos.position, transform.rotation);

        // 일정 시간이 지난 후 삭제
        Destroy(_bullet, 3.0f);

        // 남은 총알로 재장전 여부 계산
        isReload = (--currentBullet % maxBullet == 0);

        if (isReload)
        {
            StartCoroutine(Reloading());
        }
    }

    IEnumerator Reloading()
    {
        // 재장전 애니메이션 실행
        animator.SetTrigger(hashReload);

        // 재장던 사운드 재생
        audioSource.PlayOneShot(reloadSFX, 1.0f);

        // 재장전 시간만큼 대기하는 동안 제어권 양보
        yield return wsReload;

        // 총알 갯수 초기화
        currentBullet = maxBullet;
        isReload = false;
    }
}
