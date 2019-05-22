using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float scale = 1f;

    const float viewerMoveTresholdForChunkUpdate = 25;
    const float sqrViewerMoveTresholdForChunkUpdate = viewerMoveTresholdForChunkUpdate * viewerMoveTresholdForChunkUpdate;

    public static float maxViewDistance = 512;
    public LODInfo[] detailLevels;

    public Transform viewer;
    public Material mapMaterial;
    public bool generateCollision = false;
    int chunkSize;
    int chunksVisibleInView;

    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    static MapGenerator mapGenerator;

    public Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    // Start is called before the first frame update
    void Start()
    {
        chunkSize = MapGenerator.chunkSize - 1;

        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDstTreshold;
        chunksVisibleInView = Mathf.RoundToInt(maxViewDistance / chunkSize);
        mapGenerator = FindObjectOfType<MapGenerator>();

        UpdateVisibleChunks();
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
                Vector2 currentViewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset); 
                if (terrainChunkDictionary.ContainsKey(currentViewedChunkCoord))
                {
                    terrainChunkDictionary[currentViewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDictionary.Add(currentViewedChunkCoord, new TerrainChunk(currentViewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;
        //TODO: don't need to tick it every frame i think
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveTresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
            
    }

    public class TerrainChunk
    {
        public Vector2 position;
        public MeshData meshDataForTileFinder;

        GameObject meshObejct;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;
        LODMesh collisionLODMesh;

        float[,] noiseMap;
        bool noiseMapRecieved;
        int prevLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;

            position = coord * size;
            bounds = new Bounds(position, Vector3.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0 , position.y);

            meshObejct = new GameObject("Terrain Chunk");
            meshRenderer = meshObejct.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshFilter = meshObejct.AddComponent<MeshFilter>();
            meshCollider = meshObejct.AddComponent<MeshCollider>();

            
            meshObejct.layer = 9;
            meshObejct.transform.position = positionV3 * scale;
            meshObejct.transform.parent = parent;
            meshObejct.transform.localScale = Vector3.one * scale;

            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];

            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
                if (detailLevels[i].useForCollider)
                {
                    collisionLODMesh = lodMeshes[i];
                }
            }

            mapGenerator.RequestNosieMap(position, OnNoiseMapRecieved);
        }

        void OnNoiseMapRecieved(float[,] noiseMap)
        {
            this.noiseMap = noiseMap;
            noiseMapRecieved = true;

            Texture2D texture = MapTexture.GenerateTextureFromNoiseMap(noiseMap);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
        }

        void OnMeshDataForTileFinderRecieved(MeshData meshData)
        {
            meshDataForTileFinder = meshData;
        }

        public void UpdateTerrainChunk()
        {
            if (noiseMapRecieved)
            {
                float viewerSqrDistnaceFromNearestEdge = bounds.SqrDistance(viewerPosition);
                bool visible = viewerSqrDistnaceFromNearestEdge < maxViewDistance * maxViewDistance;

                if (visible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerSqrDistnaceFromNearestEdge > detailLevels[i].visibleDstTreshold * detailLevels[i].visibleDstTreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (prevLODIndex != lodIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            meshFilter.mesh = lodMesh.mesh;
                            prevLODIndex = lodIndex;
                            
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            if (lodIndex == 0)
                                lodMesh.RequestMesh(noiseMap, OnMeshDataForTileFinderRecieved);
                            else
                                lodMesh.RequestMesh(noiseMap, null);
                        }

                    }

                    if (lodIndex == 0)
                    {
                        if (collisionLODMesh.hasMesh)
                        {
                            meshCollider.sharedMesh = collisionLODMesh.mesh;
                        }
                        else if (!collisionLODMesh.hasRequestedMesh)
                        {
                            collisionLODMesh.RequestMesh(noiseMap, null);
                        }
                    }

                    terrainChunksVisibleLastUpdate.Add(this);
                }
                SetVisible(visible);
            }
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

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        public int lod;
        System.Action updateCallback;
        System.Action<MeshData> onMeshDataRecievedCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataRecieved(MeshData data)
        {
            mesh = data.CreateMesh();
            hasMesh = true;
            if (onMeshDataRecievedCallback != null)
                onMeshDataRecievedCallback(data);
            updateCallback();
        }

        public void RequestMesh(float[,] noiseMap, System.Action<MeshData> onMeshDataRecieved)
        {
            hasRequestedMesh = true;
            onMeshDataRecievedCallback = onMeshDataRecieved;
            mapGenerator.RequestMeshData(noiseMap, lod, OnMeshDataRecieved);
        }

    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstTreshold;
        public bool useForCollider;
    }
}
