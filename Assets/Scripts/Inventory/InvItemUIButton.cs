using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvItemUIButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    public ItemInvEntry myEntry;
    private Vector3 lastPos;
    RectTransform rectTransform;
    CanvasGroup canvasGroup;

    private void Start()
    {
        lastPos = new Vector3();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //transform.position = lastPos;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPos = transform.position;
        canvasGroup.blocksRaycasts = false;
    }
}
