using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvItemUIButton : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public ItemInvEntry myEntry;
    public GenericInventory currentInv;

    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public Image graphics;
    public Text countText;

    private Vector3 lastPos;

    public System.Action<ItemInvEntry> OnPickup;
    public System.Action<ItemInvEntry> OnDrop;

    public void UpdateUI()
    {
        graphics.sprite = myEntry.itemData.sprite;
        countText.text = myEntry.count.ToString();
        rectTransform.sizeDelta = new Vector2(myEntry.itemData.size.x * 75, myEntry.itemData.size.y * 75);
    }

    private void Start()
    {
        lastPos = new Vector3();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        OnDrop?.Invoke(myEntry);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPos = transform.position;
        canvasGroup.blocksRaycasts = false;
        if (OnPickup != null)
            OnPickup(myEntry);
    }
}
