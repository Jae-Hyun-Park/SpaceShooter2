using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour {

    // 폭발 이펙트 프리팹 
    public GameObject expEffect;

    // 찌그러진 드럼통의 메쉬를 저장할 배열
    public Mesh[] meshes;

    // 드럼통의 텍스쳐들을 저장할 배열
    public Texture[] textures;

    // 폭발 반경
    public float expRadius = 10.0f;

	// 폭발 효과음
	AudioSource audioSource;

	// 폭발 오디오 클립
	public AudioClip expSfx;

    // 총알이 맞은 횟수
    int hitCount = 0;

    // Rigbody Component 인스턴스
    Rigidbody rb;

    // 실제 렌더링 할 Mesh인스턴스 
    MeshFilter meshFilter;

    // MeshRenderer 컴포넌트 인스턴스
    MeshRenderer meshRenderer;

    Shaker shaker;

	// Use this for initialization
	void Start () {

        // 리지드바디 컴포넌트 인스턴스 연결
        rb = GetComponent<Rigidbody>();

        // 메쉬필터 컴포넌트 인스턴스 연결
        meshFilter = GetComponent<MeshFilter>();

        // 메쉬렌더러 컴포넌트 인스턴스 연결
        meshRenderer = GetComponent<MeshRenderer>();

        // 메쉬렌더러의 머터리얼이 참조하는 텍스쳐를 textures배열에서 랜덤하게 하나로 변경
        meshRenderer.material.mainTexture = textures[Random.Range(0, textures.Length)];

		audioSource = GetComponent<AudioSource>();

        shaker = GameObject.Find("CameraRig").GetComponent<Shaker>();
    }

    // 충돌 발생시 한번 호출되는 콜백 함수
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("BULLET"))
        {
            // 총알의 충돌 횟수 증가
            if(++hitCount == 3)
            {
                // 여기는 총알에 맞아 폭발하는 것이므로
                // 자기 자신의 포지션을 전달인자로
                StartExpBarrel(transform.position, 1.0f);
            }
        }
    }
    
    public void StartExpBarrel(Vector3 pos, float delay)
    {
        StartCoroutine(ExpBarrel(pos, delay));
    }
    IEnumerator ExpBarrel(Vector3 pos, float delayTime)
    {
        if (hitCount >= 4)
            yield break;

        yield return new WaitForSeconds(delayTime);

        shaker.StartCoroutine(shaker.ShakeCamera(0.1f, 0.2f, 0.5f));

		audioSource.PlayOneShot(expSfx, 1.0f);
        ChangeAfterExp();
        // 폭발 방향을 랜덤하게 정하는 로직
        //Vector3 direction = Random.insideUnitSphere;
        //direction = new Vector3(
        //    Mathf.Clamp(direction.x, -0.5f, 0.5f),
        //    1.0f, 
        //    Mathf.Clamp(direction.z, -0.5f, 0.5f));

        //rb.AddForce(Vector3.up * 1000.0f);

        // 주변에 폭발을 전달한다.
        IndirectDamage(pos);
    }

    public void ChangeAfterExp()
    {
        hitCount = 4;

        // 폭발 효과 프리팹을 동적으로 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.0f);

        // 0 ~ meshes 배열의 길이까지 (0 ~ meshes.Length - 1)
        int idx = Random.Range(0, meshes.Length);

        // 찌그러진 메쉬를 적용
        meshFilter.sharedMesh = meshes[idx];

        // 드럼통의 질량을 가볍게 한다.
        rb.mass = 1.0f;
    }

    // 폭발을 주변에 전달하는 함수
    void IndirectDamage(Vector3 pos)
    {
        // 폭발 반경 내에 있는 8번 레이어에 해당하는 모든 Collider를 배열에 담아온다.
        Collider[] colliders = Physics.OverlapSphere(pos, expRadius, 
            1 << LayerMask.NameToLayer("BARREL") );

        foreach(Collider collider in colliders)
        {
            // 폭발 범위에 포함된 드럼통에 Rigidbody 컴포넌트 추출
            var barrel = collider.GetComponent<BarrelCtrl>();

            Rigidbody rigidBody = barrel.GetComponent<Rigidbody>();
            rigidBody.mass = 1.0f;

            // 폭발하는 힘을 추가 한다.
            rigidBody.AddExplosionForce(500.0f, pos, expRadius, 400.0f);

            barrel.StartExpBarrel(barrel.transform.position, 1.0f);
        }
    }

}
