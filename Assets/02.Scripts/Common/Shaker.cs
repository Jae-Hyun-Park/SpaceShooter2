using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour {

    // Shake 효과를 줄 카메라 Transform을 저장할 변수
    public Transform shakeCamera;

    // 회전시킬 것인지 아닌지를 판단할 변수
    public bool shakeRotate = false;

    // 초기값을 저장할 변수
    private Vector3 originPos;          // 위치
    private Quaternion originRot;     // 회전
	// Use this for initialization
	void Start () {
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
	}
	
    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float magnitudeRot = 0.1f)
    {
        // 지나간 시간을 누적해 저장할 변수
        float elapsedTime = 0.0f;

        // 진동시간 동안 루프를 돈다.
        while (elapsedTime < duration)
        {
            // 불규칙하게 위치를 산출
            Vector3 shakePos = Random.insideUnitSphere;

            // 카메라의 위치 변경
            shakeCamera.localPosition = shakePos * magnitudePos;

            if (shakeRotate)
            {
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));

                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;
    }
}
