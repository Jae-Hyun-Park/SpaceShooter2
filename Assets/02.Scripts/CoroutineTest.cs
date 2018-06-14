using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Fade());
	}
	
	// Update is called once per frame
	void Update () {

        
	}

    IEnumerator Fade()
    {
        Color c = GetComponent<MeshRenderer>().material.GetColor("_TintColor");

        Debug.Log("초기 알파값 : " + c.a);

        float startTime = Time.time;

        while (c.a > 0.01f)
        {
            c.a -= 0.1f;
            GetComponent<MeshRenderer>().material.SetColor("_TintColor", c);
            yield return new WaitForSeconds(0.1f);
            Debug.Log("코루틴 처리 시간 : " + (Time.time - startTime));
        }
        Debug.Log("Fade Finished!");
    }
}
