using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBuildingElement : MonoBehaviour
{
    public StaticBuildingElement parent;
    public StaticBuildingElement[] children;

    //Redundant, can get from mesh
    public Bounds bounds;
    //Redundant can get from transform
    public Vector3 position;
}
