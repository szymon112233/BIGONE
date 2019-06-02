using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementSize
{
    Small = 2,
    Medium = 4,
    Large = 6,
    Huge = 8
}

public class StaticBuildingElement : MonoBehaviour
{
    public StaticBuildingElement parent;
    public StaticBuildingElement[] children;

    public ElementSize size;

    //Redundant, can get from mesh
    public Bounds bounds;
    //Redundant can get from transform
    public Vector3 position;
}
