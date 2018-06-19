using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour {

    private const string bulletTag = "BULLET";

    // 체력
    private float hp = 100.0f;

    // 피격시 사용 할 혈흔 이펙트
    private GameObject bloodEffect;

    // Use this for initialization
	void Start () {
        // 혈흔 효과 프리팹을 리소스 폴더에서 로드
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == bulletTag)
        {
            // 혈흔 효과 출력
            ShowBloodEffect(collision);

            // 총알 삭제
            Destroy(collision.gameObject);

            // 생명력 차감
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;

            if (hp < 0.0f)
            {
                // Enemy상태를 DIE로 변경
                GetComponent<EnemyAI>().state = EnemyAI.EnemyState.DIE;
            }
        }
    }

    void ShowBloodEffect(Collision collision)
    {
        // 총알이 충돌한 지점 계산
        Vector3 pos = collision.contacts[0].point;

        // 총알이 충돌했을때의 법선 벡터
        Vector3 normal = collision.contacts[0].normal;

        // 총알 충돌시 방향 벡터에 따른 회전 정보 계산
        Quaternion rotation = Quaternion.FromToRotation(-Vector3.forward, normal);

        // 혈은 효과 생성
        GameObject blood = Instantiate(bloodEffect, pos, rotation);
        Destroy(blood, 1.0f);
    }
}
