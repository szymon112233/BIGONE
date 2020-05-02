using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ItemInvEntry
{
    public GenericInvItem itemData = null;
    public int count = 1;
    public Vector2Int position = new Vector2Int();
    public Vector2Int currentInvSize = new Vector2Int();
    public GameObject UIgameObject;
    public int myInvKey = -1;
}

[System.Serializable]
public class InventorySerializableData
{
    public Vector2Int size;
    public List<ItemInvEntry> items;
}

public class GenericInventory : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public List<ItemInvEntry> items;
    public GameObject itemGraphicsPrefab;
    public RectTransform graphicsParent;
    public RectTransform borderRectTransform;
    public RectTransform backgroundRectTransform;
    public RectTransform gridRectTransform;
    public ItemInvEntry myOwningContainerItem;
    public GameObject placeHelper;
    public GameObject closeButton;
    public Vector2Int inventorySize;
    public bool closable;
    public bool movable;
    private bool[,] slotsOccupancy;
    private bool isCursorOver;

    public static ItemInvEntry currentlyDraggedItem;

    private void Awake()
    {
        Init(inventorySize, Vector2.zero, null, closable, movable);
        borderRectTransform.GetComponent<DragUtilityWrapper>().OnDragEvent += onDrag;

    }

    private void OnDestroy() 
    {
        borderRectTransform.GetComponent<DragUtilityWrapper>().OnDragEvent -= onDrag;
    }

    public void Init(Vector2Int size,Vector2 position, ItemInvEntry creator = null, bool closable = false, bool movable = false)
    {
        inventorySize = size;
        if (position != Vector2.zero)
        graphicsParent.position = position;
        if (creator != null)
            myOwningContainerItem = creator;
        this.closable = closable;
        this.movable = movable;
        ClearItems();
        closeButton.SetActive(closable);
        graphicsParent.sizeDelta = new Vector2(size.x * 75, size.y * 75);
        borderRectTransform.sizeDelta = new Vector2(size.x * 75 * 1.122f, size.y * 75 * 1.122f);
        backgroundRectTransform.sizeDelta = new Vector2(size.x * 75, size.y * 75);
        gridRectTransform.sizeDelta = new Vector2(size.x * 75, size.y * 75);
        gridRectTransform.GetComponent<RawImage>().uvRect = new Rect(Vector2.zero, new Vector2(size.x, size.y));
    }

    private void ClearItems()
    {
        items = new List<ItemInvEntry>();
        slotsOccupancy = new bool[inventorySize.x, inventorySize.y];
    }

    public bool LoadInventory(int invKey)
    {
        InventorySerializableData data;
        if (!Globals.Instance.inventorySaveDataManager.GetInventory(invKey, out data))
            return false;
        inventorySize = data.size;
        foreach (ItemInvEntry item in data.items)
        {
            if (!CreateItem(item))
                return false;
        }
        return true;
    }

    public InventorySerializableData SerializeInventory()
    {
        InventorySerializableData data = new InventorySerializableData();
        data.items = items;
        data.size = inventorySize;

        return data;
    }

    bool isSpaceFree(Vector2Int position, Vector2Int size)
    {
        for (int x = position.x; x < position.x + size.x; x++)
        {
            for (int y = position.y; y < position.y + size.y; y++)
            {
                Debug.LogFormat("isSpaceFree:({0},{1})", x, y);
                if (x > inventorySize.x -1 || y > inventorySize.y -1)
                    return false;
                if (slotsOccupancy[x, y])
                    return false;
            }
        }

        return true;
    }

    void MarkSlotsOccupied(Vector2Int position, Vector2Int size, bool occupied = true)
    {
        for (int x = position.x; x < position.x + size.x; x++)
        {
            for (int y = position.y; y < position.y + size.y; y++)
            {
                slotsOccupancy[x, y] = occupied;
            }
        }
    }

    public bool AddItem(ItemInvEntry item)
    {
        Debug.LogFormat("Trying to add a new item to inventory at:({0},{1}), size:({2},{3})",
            item.position.x, 
            item.position.y, 
            item.currentInvSize.x, 
            item.currentInvSize.y);

        if (isSpaceFree(item.position, item.currentInvSize))
        {
            RectTransform itemRectTransform = item.UIgameObject.GetComponent<RectTransform>();
            itemRectTransform.SetParent(graphicsParent);
            itemRectTransform.anchoredPosition = new Vector2(item.position.x * 75, item.position.y * -75);
            item.UIgameObject.GetComponent<InvItemUIButton>().OnPickup += RemoveItem;
            item.UIgameObject.GetComponent<InvItemUIButton>().currentInv = this;
            MarkSlotsOccupied(item.position, item.currentInvSize);
            items.Add(item);
            return true;
        }
        return false;
    }

    void RemoveItem(ItemInvEntry item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            MarkSlotsOccupied(item.position, item.currentInvSize, false);
            item.UIgameObject.GetComponent<InvItemUIButton>().currentInv = null;
            item.UIgameObject.GetComponent<InvItemUIButton>().prevInv = this;
            item.UIgameObject.GetComponent<InvItemUIButton>().OnPickup -= RemoveItem;

        }
    }

    bool CreateItem(ItemInvEntry item)
    {
        item.UIgameObject = Instantiate(itemGraphicsPrefab, graphicsParent);
        item.UIgameObject.GetComponent<InvItemUIButton>().myEntry = item;
        item.UIgameObject.GetComponent<InvItemUIButton>().InitUI();
        if (AddItem(item))
        {
            return true;
        }
        else
        {
            Destroy(item.UIgameObject);
            return false;
        }
    }

    public void AddRandomItem()
    {
        GenericInvItem itemData = Globals.Instance.itemDatabase.items[Random.Range(0, Globals.Instance.itemDatabase.items.Count)];
        ItemInvEntry item = new ItemInvEntry();
        item.itemData = itemData;
        item.currentInvSize = itemData.size;
        item.position = new Vector2Int(Random.Range(0, inventorySize.x), Random.Range(0, inventorySize.y));
        item.count = Random.Range(1, 999);

        item.UIgameObject = Instantiate(itemGraphicsPrefab, graphicsParent);
        item.UIgameObject.GetComponent<InvItemUIButton>().myEntry = item;
        item.UIgameObject.GetComponent<InvItemUIButton>().InitUI();
        if (!AddItem(item))
            Destroy(item.UIgameObject);
    }

    public void AddCategoryRandomItem<T>() where T : GenericInvItem
    {
        ItemInvEntry item = new ItemInvEntry();
        if (typeof(T) == typeof(ContainerInvItem))
        {
            item.itemData = Globals.Instance.itemDatabase.containerItems[Random.Range(0, Globals.Instance.itemDatabase.containerItems.Count)];
        }
        else
        {
            item.itemData = Globals.Instance.itemDatabase.items[Random.Range(0, Globals.Instance.itemDatabase.items.Count)];
        }
            
        item.currentInvSize = item.itemData.size;
        item.position = new Vector2Int(Random.Range(0, inventorySize.x), Random.Range(0, inventorySize.y));
        item.count = Random.Range(1, 999);

        item.UIgameObject = Instantiate(itemGraphicsPrefab, graphicsParent);
        item.UIgameObject.GetComponent<InvItemUIButton>().myEntry = item;
        item.UIgameObject.GetComponent<InvItemUIButton>().InitUI();
        if (!AddItem(item))
            Destroy(item.UIgameObject);
    }

    public void CloseInventoryWindow()
    {
        if (myOwningContainerItem.myInvKey != -1)
        {
            Globals.Instance.inventorySaveDataManager.SaveInventory(myOwningContainerItem.myInvKey, SerializeInventory(), true);
        }
        else
        {
            myOwningContainerItem.myInvKey = Globals.Instance.inventorySaveDataManager.SaveNewInventory(SerializeInventory());
        }
        GameObject.Destroy(gameObject);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.GetComponent<InvItemUIButton>())
            return;
        ItemInvEntry itemEntry = eventData.pointerDrag.GetComponent<InvItemUIButton>().myEntry;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(graphicsParent, eventData.position, null, out localPoint);
        Vector2Int desiredSlot = new Vector2Int((int)localPoint.x/ 75, -(int)localPoint.y/ 75);
        Debug.LogFormat("On Drop: {0}, {1}, {2}, {3}", eventData.pointerDrag, eventData.position, localPoint, desiredSlot);
        Vector2Int lastPos = itemEntry.position;
        itemEntry.position = desiredSlot;
        if (!AddItem(itemEntry))
        {
            itemEntry.position = lastPos;
            eventData.pointerDrag.GetComponent<InvItemUIButton>().ReturnToPrevInv();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isCursorOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isCursorOver = false;
    }

    private void Update()
    {
        if (isCursorOver && currentlyDraggedItem!= null && EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.LogFormat("Yup, currentlyDraggedItem: {0}", currentlyDraggedItem.UIgameObject.name);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(graphicsParent, Input.mousePosition, null, out localPoint);
            Vector2Int desiredSlot = new Vector2Int((int)localPoint.x / 75, -(int)localPoint.y / 75);
            placeHelper.GetComponent<RectTransform>().sizeDelta = currentlyDraggedItem.currentInvSize * 75;
            placeHelper.GetComponent<RectTransform>().anchoredPosition = new Vector2(desiredSlot.x * 75, desiredSlot.y * -75);
            if (isSpaceFree(desiredSlot, currentlyDraggedItem.currentInvSize))
                placeHelper.GetComponent<Image>().color = Color.green;
            else
                placeHelper.GetComponent<Image>().color = Color.red;
        }
        placeHelper.SetActive(isCursorOver && currentlyDraggedItem != null);
    }

    private void onDrag(PointerEventData eventData)
    {
        if (movable)
            graphicsParent.position = Input.mousePosition;
    }

}
