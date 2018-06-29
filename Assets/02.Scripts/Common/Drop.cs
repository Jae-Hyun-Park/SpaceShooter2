using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;

public class Drop : MonoBehaviour, IDropHandler {

    public void OnDrop(PointerEventData eventData)
    {
       if(transform.childCount == 0)
        {
            Drag.DraggingItem.transform.SetParent(transform);

            // 슬롯에 추가된 아이템을 GameData에 추가하기 위해 AddItem 호출
            Item item = Drag.DraggingItem.GetComponent<ItemInfo>().itemData;
            GameManager.instance.AddItem(item);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
