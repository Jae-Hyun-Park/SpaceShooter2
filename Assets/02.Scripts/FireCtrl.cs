using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCtrl : MonoBehaviour {

    // 총알 프리팹
    public GameObject bulletPrefab;

    // 총알 발사 좌표
    public Transform firePos;

    // 탄피 파티클
    public ParticleSystem catrige;

    // 총구 화염 파티클
    private ParticleSystem muzzleFlash;
	// Use this for initialization
	void Start () {
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();

    }
	
	// Update is called once per frame
	void Update () {
		
        if (Input.GetMouseButtonDown(0))
        {
            // 총알 발사 함수 호출
            Fire();
        }
	}

    // 총알 발사 함수
    void Fire()
    {
        // Bullet 프리팹을 복사해 인스턴스화
        Instantiate(bulletPrefab, firePos.position, transform.rotation);
        catrige.Play();
        muzzleFlash.Play();
    }
}
