using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour {

    // 폴발 이펙트 프리팹
    public GameObject expEffect;

    // 찌그러진 드럼통의 메쉬를 저장할 배열
    public Mesh[] meshes;

    // 드럼통의 텍스처들을 저장할 배열
    public Texture[] textures;
    // 총알을 맞은 횟수
    int hitCount = 0;

    // Rigidbody Component 인스턴스
    Rigidbody rb;
    
    // 실제 렌더링 할 Mesh인스턴스
    MeshFilter meshFilter;

    // MeshFilter 컴포넌트 인스턴스
    MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
        
        // 리지드바디 컴포넌트 인스턴스 연결
        rb = GetComponent<Rigidbody>();

        // 메쉬필터 컴포넌트 인스턴스 연결
        meshFilter = GetComponent<MeshFilter>();
        
        // 메쉬 렌더러 컴포넌트 인스턴스 연결
        meshRenderer = GetComponent<MeshRenderer>();

        // 메쉬 렌더러의 머터리얼이 참조하는 텍스쳐를 textures배열에서 랜덤하게 하나로 변경
        meshRenderer.material.mainTexture = textures[Random.Range(0, textures.Length)];

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
        direction = new Vector3(Mathf.Clamp(direction.x, -0.5f, 0.5f),
            1.0f,
            Mathf.Clamp(direction.z, -0.5f, 0.5f));

        rb.AddForce(direction * 1000.0f);

        // 0 ~ meshes 배열의 길이까지 (0 ~ meshes.Length - 1)
        meshFilter.sharedMesh = meshes[Random.Range(0, meshes.Length)];
    }
}
