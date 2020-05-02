using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : Singleton<Globals>
{
    public ItemDatabase itemDatabase;
    public GameObject InventoryTemplatePrefab;
    public InventorySaveDataManager inventorySaveDataManager;


    private void Awake()
    {
        inventorySaveDataManager = new InventorySaveDataManager();
    }
}
