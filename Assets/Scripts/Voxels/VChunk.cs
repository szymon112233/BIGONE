using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VChunk 
{
    public static Vector3Int chunkSize = new Vector3Int(32, 32, 32);
    public Mesh mesh;
    public Vector3Int position;
    public bool isMeshGenerated = false;

    VoxelType[] voxelsTypes;

    public VChunk(Vector3Int position)
    {
        this.position = position;
    }

    public void GenerateFilledChunk()
    {
        voxelsTypes = new VoxelType[chunkSize.x * chunkSize.y * chunkSize.z];

        for (int z = 0; z < chunkSize.z; z++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int x = 0; x < chunkSize.x; x++)
                {
                    voxelsTypes[z * chunkSize.y + y * chunkSize.x + x] = VoxelType.Filled;
                    Debug.Log(z * chunkSize.y + y * chunkSize.x + x);
                }
            }
        }
    }

    public IEnumerator GenerateMesh()
    {
        yield return null;
    }
}
