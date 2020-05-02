using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySaveDataManager
{
    private Dictionary<int, InventorySerializableData> savedInventories;

    public InventorySaveDataManager()
    {
        savedInventories = new Dictionary<int, InventorySerializableData>();
    }

    public bool GetInventory(int key, out InventorySerializableData invData)
    {
        if (savedInventories.ContainsKey(key))
        {
            invData = savedInventories[key];
            return true;
        }

        invData = new InventorySerializableData();
        return false;
    }

    public int SaveNewInventory(InventorySerializableData invData)
    {
        int newKey = Random.Range(0, int.MaxValue);
        //TODO: Make sure we don't have an infinite loop here...
        while (savedInventories.ContainsKey(newKey))
        {
            newKey = Random.Range(0, int.MaxValue);
        }
        savedInventories.Add(newKey, invData);

        return newKey;
    }

    public bool SaveInventory(int key, InventorySerializableData invData, bool shouldOverride = false)
    {
        if (savedInventories.ContainsKey(key))
        {
            if (shouldOverride)
            {
                savedInventories[key] = invData;
                return true;
            }
            else
            {
                Debug.LogErrorFormat("Cannot save inventory with key:{0} since it already existst! Set shouldOverride to true to override it.");
                return false;
            }
        }
        else
        {
            savedInventories[key] = invData;
            return true;
        }
    }

    public bool LoadInventories()
    {
        return false;
    }

    public bool SaveInventories()
    {
        return false;
    }
}
