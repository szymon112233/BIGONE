using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericInvItem 
{
    public string name;
    public int id;
    public Sprite sprite;
    public Vector2Int size = new Vector2Int(1, 1);
    [field: SerializeField]
    public virtual bool Stackable { get; set; }
    [field: SerializeField]
    public virtual int maxStackSize { get; set; }
}

[System.Serializable]
public class ContainerInvItem : GenericInvItem
{
    public override bool Stackable
    {
        get
        {
            return false;
        }
        set
        {

        }       
    }

    public override int maxStackSize
    {
        get
        {
            return 1;
        }
        set
        {

        }
    }

    public Vector2Int myInvSize;
}
