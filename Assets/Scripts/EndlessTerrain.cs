using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDistance = 512;
    public Transform viewer;
    int chunkSize;
    int chunksVisibleInView;

    public static Vector2 viewerPosition;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    // Start is called before the first frame update
    void Start()
    {
        chunkSize = MapGenerator.chunkSize;
        chunksVisibleInView = Mathf.RoundToInt(maxViewDistance / chunkSize);
    }


    public void UpdateVisibleChunks()
    {
        foreach (TerrainChunk terrainChunk in terrainChunksVisibleLastUpdate)
        {
            terrainChunk.SetVisible(false);
        }

        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for(int yOffset = -chunksVisibleInView ; yOffset <= chunksVisibleInView; yOffset++ )
        {
            for (int xOffset = -chunksVisibleInView; xOffset <= chunksVisibleInView; xOffset++)
            {
                Vector2 currentViewedChunkCoord = new Vector2(currentChunkCoordX + xOffset,currentChunkCoordY + yOffset); 
                if (terrainChunkDictionary.ContainsKey(currentViewedChunkCoord))
                {
                    terrainChunkDictionary[currentViewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[currentViewedChunkCoord].isVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[currentViewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(currentViewedChunkCoord, new TerrainChunk(currentViewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        //TODO: don't need to tick it every frame i think
        UpdateVisibleChunks();
    }

    public class TerrainChunk
    {
        GameObject meshObejct;
        Vector2 position;
        Bounds bounds;

        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector3.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0 , position.y);

            meshObejct = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObejct.transform.position = positionV3;
            meshObejct.transform.localScale = Vector3.one * size / 10.0f;//Magic number: Unity's default plane size is 10 units;
            meshObejct.transform.parent = parent;

            SetVisible(false);
        }

        public void UpdateTerrainChunk()
        {
            float viewerSqrDistnaceFromNearestEdge = bounds.SqrDistance(viewerPosition);
            bool visible = viewerSqrDistnaceFromNearestEdge < maxViewDistance * maxViewDistance;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObejct.SetActive(visible);
        }

        public bool isVisible()
        {
            return meshObejct.activeSelf;
        }
    }
}
