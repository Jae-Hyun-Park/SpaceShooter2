using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Drop : MonoBehaviour, IDropHandler {

    public void OnDrop(PointerEventData eventData)
    {
       if(transform.childCount == 0)
        {
            Drag.DraggingItem.transform.SetParent(transform);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
