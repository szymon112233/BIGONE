using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBuildingElement : MonoBehaviour
{
    public StaticBuildingElement parent;
    public StaticBuildingElement[] children;

    public Vector3 position;
}
