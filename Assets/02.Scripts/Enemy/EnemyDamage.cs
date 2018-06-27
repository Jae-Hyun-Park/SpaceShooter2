using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour {

    private const string bulletTag = "BULLET";

    // 체력
    private float hp = 100.0f;

    private float maxHP = 100.0f;

    // 피격시 사용 할 혈흔 이펙트
    private GameObject bloodEffect;

    public GameObject hpBarPrefab;

    public Vector3 hpBarOffset = new Vector3(0.0f, 2.2f, 0.0f);

    private Canvas uiCanvas;

    private Image hpBarImage;

    // Use this for initialization
	void Start () {
        // 혈흔 효과 프리팹을 리소스 폴더에서 로드
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");

        uiCanvas = GameObject.Find("Canvas-Camera").GetComponent<Canvas>();

        SetHPBar();
	}
	
    void SetHPBar()
    {
        GameObject hpBar = Instantiate(hpBarPrefab, uiCanvas.transform);

        Image[] images = hpBar.GetComponentsInChildren<Image>();

        foreach(Image img in images)
        {
            if (img.transform != hpBar.transform)
            {
                hpBarImage = img;
                break;
            }
        }
        var enemyHPBar = hpBar.GetComponent<EnemyHPBar>();
        enemyHPBar.enemyTransform = transform;
        enemyHPBar.offset = hpBarOffset;
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
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);

            // 생명력 차감
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;

            hpBarImage.fillAmount = hp / maxHP;

            if (hp < 0.0f)
            {
                // Enemy상태를 DIE로 변경
                GetComponent<EnemyAI>().state = EnemyAI.EnemyState.DIE;
                GetComponent<Collider>().enabled = false;

                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;
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
