using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvItemUIButton : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public ItemInvEntry myEntry;
    public GenericInventory currentInv;
    public GenericInventory prevInv;

    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public Image graphics;
    public TMPro.TextMeshProUGUI countText;

    public System.Action<ItemInvEntry> OnPickup;
    public System.Action<ItemInvEntry> OnDrop;

    public void UpdateUI()
    {
        graphics.sprite = myEntry.itemData.sprite;
        countText.SetText(myEntry.count.ToString());
        rectTransform.sizeDelta = new Vector2(myEntry.itemData.size.x * 75, myEntry.itemData.size.y * 75);
    }

    public void ReturnToPrevInv()
    {
        if (prevInv)
            prevInv.AddItem(myEntry);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GenericInventory.currentlyDraggedItem = null;
        canvasGroup.blocksRaycasts = true;
        if (currentInv == null)
            ReturnToPrevInv();
        OnDrop?.Invoke(myEntry);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GenericInventory.currentlyDraggedItem = myEntry;
        canvasGroup.blocksRaycasts = false;
        if (OnPickup != null)
            OnPickup(myEntry);
    }
}
