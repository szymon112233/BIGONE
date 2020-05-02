using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class DragUtilityWrapper : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public System.Action<PointerEventData> OnBeginDragEvent;
    public System.Action<PointerEventData> OnDragEvent;
    public System.Action<PointerEventData> OnEndDragEvent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragEvent != null)
            OnBeginDragEvent(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragEvent != null)
            OnDragEvent(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragEvent != null)
            OnEndDragEvent(eventData);
    }
}
