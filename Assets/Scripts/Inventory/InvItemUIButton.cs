using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvItemUIButton : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler
{
    public ItemInvEntry myEntry;
    public GenericInventory currentInv;
    public GenericInventory prevInv;

    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public Image graphics;
    public TMPro.TextMeshProUGUI countText;
    public Canvas dragCanvas;
    public GameObject myInventoryGO;

    public System.Action<ItemInvEntry> OnPickup;
    public System.Action<ItemInvEntry> OnDrop;

    bool isDragged;
    private float lastClickTime;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("DragCanvas").GetComponent<Canvas>())
            dragCanvas = GameObject.FindGameObjectWithTag("DragCanvas").GetComponent<Canvas>();
        else
            Debug.LogErrorFormat("{0}: Could not find a canvas to be attached to :(", gameObject.name);
        if (myEntry != null)
            myEntry.currentInvSize = myEntry.itemData.size;
    }

    public void InitUI()
    {
        gameObject.name = string.Format("{0}x{1}", myEntry.itemData.name, myEntry.count);
        graphics.sprite = myEntry.itemData.sprite;
        if (myEntry.itemData.Stackable)
            countText.SetText(myEntry.count.ToString());
        else
            countText.gameObject.SetActive(false);
        rectTransform.sizeDelta = new Vector2(myEntry.currentInvSize.x * 75, myEntry.currentInvSize.y * 75);
        graphics.rectTransform.sizeDelta = rectTransform.sizeDelta;
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

    private void Update()
    {
        if (isDragged && Input.GetKeyDown(KeyCode.R))
            Rotate();

    }

    void Rotate()
    {
        myEntry.currentInvSize = new Vector2Int(myEntry.currentInvSize.y, myEntry.currentInvSize.x);
        rectTransform.sizeDelta = new Vector2(myEntry.currentInvSize.x * 75, myEntry.currentInvSize.y * 75);
        //if we have original rotation (2,4)
        if (myEntry.currentInvSize == myEntry.itemData.size)
        {
            graphics.rectTransform.localRotation = Quaternion.AngleAxis(0, Vector3.forward);
            graphics.rectTransform.anchoredPosition = new Vector2(0, 0);
        }
        //(1,3)
        else
        {
            //if horizontal (1)
            if (myEntry.itemData.size.x > myEntry.itemData.size.y)
            {
                graphics.rectTransform.localRotation = Quaternion.AngleAxis(-90, Vector3.forward);
                graphics.rectTransform.anchoredPosition = new Vector2(75 * myEntry.itemData.size.y, 0);
            }
            // (3)
            else if (myEntry.itemData.size.x < myEntry.itemData.size.y)
            {
                graphics.rectTransform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
                graphics.rectTransform.anchoredPosition = new Vector2(0, -75 * myEntry.itemData.size.x);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GenericInventory.currentlyDraggedItem = null;
        canvasGroup.blocksRaycasts = true;
        isDragged = false;
        if (currentInv == null)
            ReturnToPrevInv();
        OnDrop?.Invoke(myEntry);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GenericInventory.currentlyDraggedItem = myEntry;
        canvasGroup.blocksRaycasts = false;
        if (dragCanvas)
            rectTransform.SetParent(dragCanvas.transform);
        isDragged = true;
        if (OnPickup != null)
            OnPickup(myEntry);
    }

    void OnSingleClick()
    {
        Debug.Log("Single click!");
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        float clickTimeDelta = eventData.clickTime - lastClickTime;
        lastClickTime = eventData.clickTime;

        Debug.LogFormat("clickTimeDelta: {0}, clickCount: {1}", clickTimeDelta, eventData.clickCount);
        if (/*eventData.clickCount >= 2 && */clickTimeDelta < 0.55f)
        {
            OnDoubleClick();
        }
        else
        {
            OnSingleClick();
        }
    }

    private void OnDoubleClick()
    {

        Debug.Log("Double Click");
        if (myEntry.itemData.GetType() == typeof(ContainerInvItem))
        {
            OpenMyInventory();
        }
    }

    private void OpenMyInventory()
    {
        if (myInventoryGO == null)
        {
            myInventoryGO = Instantiate(Globals.Instance.InventoryTemplatePrefab);
            ContainerInvItem containerInfo = (ContainerInvItem)myEntry.itemData;
            GenericInventory inv = myInventoryGO.GetComponent<GenericInventory>();
            inv.Init(containerInfo.myInvSize, Input.mousePosition, myEntry, true, true);    
            inv.LoadInventory(myEntry.myInvKey);
        }
    }
}
