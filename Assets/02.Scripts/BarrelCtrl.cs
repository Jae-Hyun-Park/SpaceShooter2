using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour {

    // 폴발 이펙트 프리팹
    public GameObject expEffect;

    // 총알을 맞은 횟수
    int hitCount = 0;

    // Rigidbody Component 인스턴스
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();	
	}

    // 충돌 발생시 한번 호출되는 콜백 함수
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            if (++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        // 폭발 효과 프리팹을 동적으로 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.0f);

        // Rigidbody의 mass를 1.0으로 수정 -> 무게를 가볍게 만듬
        rb.mass = 1.0f;

        Vector3 direction = Random.insideUnitSphere;
        direction = new Vector3(direction.x, Mathf.Abs(direction.y), direction.z);
        rb.AddForce(direction * 1000.0f);
    }
}
