using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    private Transform itemTransform;
    private Transform inventoryTransform;
    private Transform itemListTransform;
    private CanvasGroup canvasGroup;

    public static GameObject DraggingItem = null;
	// Use this for initialization
	void Start () {
        itemTransform = GetComponent<Transform>();
        inventoryTransform = GameObject.Find("Inventory").GetComponent<Transform>();
        itemListTransform = GameObject.Find("ItemList").GetComponent<Transform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        itemTransform.position = Input.mousePosition;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(inventoryTransform);
        DraggingItem = gameObject;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DraggingItem = null;
        canvasGroup.blocksRaycasts = true;

        if(itemTransform.parent == inventoryTransform)
        {
            itemTransform.SetParent(itemListTransform.transform);
            GameManager.instance.RemoveItem(GetComponent<ItemInfo>().itemData);
        }
    }
}
