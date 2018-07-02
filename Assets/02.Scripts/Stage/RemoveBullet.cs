using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour {

    // 스파크 프리팹 인스턴스를 저장할 변수
    public GameObject sparkEffect;

    // 스파크 이펙트의가 이 오브젝트의 자식으로 붙일것인지 체크하는 변수
    public bool isSparkEffectRelative;

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "BULLET")
        {
            ShowEffect(collider);
            //Destroy(collision.gameObject);
            collider.gameObject.SetActive(false);
        }
    }
   
    void ShowEffect(Collider collider)
    {
        // 충돌 지점의 정보를 추출
        Vector3 pos = collider.transform.position;

        Vector3 normal = collider.transform.forward;

        // 충돌지점의 법선 벡터가 이루는 회전각도를 추출
        Quaternion rotation = Quaternion.FromToRotation(Vector3.back, normal);

        // 스파크 효과를 생성
        Instantiate(sparkEffect,
            pos,
            rotation,
            isSparkEffectRelative ? transform : null);
    }
}
