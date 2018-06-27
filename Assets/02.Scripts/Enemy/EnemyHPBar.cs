using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour {

    // RenderMode Camera 방식으로 UI만을 그릴 카메라 Component
    private Camera uiCamera;

    // Camera모드로 그리는 Canvas의 Component
    private Canvas canvas;

    private RectTransform rectParent;

    private RectTransform rectTransform;

    [HideInInspector] public Vector3 offset = Vector3.zero;

    [HideInInspector] public Transform enemyTransform;
    
    // Use this for initialization
    void Start () {
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void LateUpdate () {

        var screenPos = Camera.main.WorldToScreenPoint(enemyTransform.position + offset);

        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
        }

        var localPos = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        rectTransform.localPosition = localPos;
	}
}
