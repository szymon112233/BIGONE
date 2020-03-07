using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class ItemInvEntry
{
    public GenericInvItem itemData = null;
    public int count = 1;
    public bool isHorizontal = true;
    public Vector2Int position = new Vector2Int();
    public GameObject UIgameObject;
}

public class GenericInventory : MonoBehaviour, IDropHandler
{
    public List<ItemInvEntry> items;
    public GameObject itemGraphicsPrefab;
    public Transform graphicsParent;
    private Vector2Int inventorySize;
    private bool[,] slotsOccupancy;

    private void Awake()
    {
        items = new List<ItemInvEntry>();
        inventorySize = new Vector2Int(8, 8);
        slotsOccupancy = new bool[inventorySize.x, inventorySize.y];
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
            item.itemData.size.x, 
            item.itemData.size.y);

        if (isSpaceFree(item.position, item.itemData.size))
        {
            RectTransform itemRectTransform = item.UIgameObject.GetComponent<RectTransform>();
            itemRectTransform.parent = gameObject.transform;
            itemRectTransform.anchoredPosition = new Vector2(item.position.x * 75, item.position.y * -75);
            item.UIgameObject.GetComponent<InvItemUIButton>().OnPickup += RemoveItem;
            item.UIgameObject.GetComponent<InvItemUIButton>().currentInv = this;
            MarkSlotsOccupied(item.position, item.itemData.size);
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
            MarkSlotsOccupied(item.position, item.itemData.size, false);
            item.UIgameObject.GetComponent<InvItemUIButton>().OnPickup -= RemoveItem;

        }
    }

    public void AddRandomItem()
    {
        GenericInvItem itemData = Globals.Instance.itemDatabase.items[Random.Range(0, Globals.Instance.itemDatabase.items.Count)];
        ItemInvEntry item = new ItemInvEntry();
        item.itemData = itemData;
        item.position = new Vector2Int(Random.Range(0, 8), Random.Range(0, 8));

        item.UIgameObject = Instantiate(itemGraphicsPrefab, graphicsParent);
        item.UIgameObject.GetComponent<InvItemUIButton>().myEntry = item;
        item.UIgameObject.GetComponent<InvItemUIButton>().UpdateUI();
        if (!AddItem(item))
            Destroy(item.UIgameObject);
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemInvEntry itemEntry = eventData.pointerDrag.GetComponent<InvItemUIButton>().myEntry;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, null, out localPoint);
        Vector2Int desiredSlot = new Vector2Int((int)localPoint.x/ 75, -(int)localPoint.y/ 75);
        Debug.LogFormat("On Drop: {0}, {1}, {2}, {3}", eventData.pointerDrag, eventData.position, localPoint, desiredSlot);
        Vector2Int lastPos = itemEntry.position;
        itemEntry.position = desiredSlot;
        if (!AddItem(itemEntry))
        {
            itemEntry.position = lastPos;
            eventData.pointerDrag.GetComponent<InvItemUIButton>().currentInv.AddItem(itemEntry);
        }
    }
}
