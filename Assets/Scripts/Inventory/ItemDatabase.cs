using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemDatabase : ScriptableObject
{
    public List<GenericInvItem> items;
    public List<ContainerInvItem> containerItems;
}
