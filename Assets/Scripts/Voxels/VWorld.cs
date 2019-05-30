using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoxelType : ushort
{
    Air = 0,
    Filled = 1
}

public class VWorld : MonoBehaviour
{

    public Dictionary<Vector3Int, VChunk>  chunksDictionary;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            
    }

}
