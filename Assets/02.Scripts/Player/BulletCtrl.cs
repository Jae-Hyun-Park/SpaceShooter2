using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour {

    // 총알의 데미지
    public float damage = 20.0f;

    // 총알의 속도
    public float speed = 1000.0f;

    Rigidbody rb;
    Transform tr;
    TrailRenderer trail;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        trail = GetComponent<TrailRenderer>();
        damage = GameManager.instance.gameData.damage;
    }
    // Use this for initialization
    void OnEnable () {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
	}

    private void OnDisable()
    {
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }
    // Update is called once per frame
    void Update () {
		
	}
}
