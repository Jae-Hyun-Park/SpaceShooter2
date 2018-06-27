using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour {

    // 스파크 프리팹 인스턴스를 저장할 변수
    public GameObject sparkEffect;

    // 스파크 이펙트의가 이 오브젝트의 자식으로 붙일것인지 체크하는 변수
    public bool isSparkEffectRelative;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "BULLET")
        {
            ShowEffect(collision);
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }
    
    void ShowEffect(Collision coll)
    {
        // 충돌 지점의 정보를 추출
        ContactPoint contact = coll.contacts[0];

        // 충돌지점의 법선 벡터가 이루는 회전각도를 추출
        Quaternion rotation = Quaternion.FromToRotation(Vector3.back, contact.normal);

        // 스파크 효과를 생성
        Instantiate(sparkEffect,
            contact.point,
            rotation,
            isSparkEffectRelative ? transform : null);
    }
}
