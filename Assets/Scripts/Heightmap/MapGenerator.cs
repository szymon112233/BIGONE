using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, TerrainMap, Mesh};
    public DrawMode currentDrawMode;

    public TerrainData terrainData;
    public NoiseData noiseData;

    public const int chunkSize = 241;
    public bool autoUpdate;
    [Range(0, 6)]
    public int editorPreviewlevelOfDetail;

    public Texture2D noiseMapInput;

    public TerrainType[] regions;


    Queue<MapThreadInfo<float[,]>> noiseThreadInfoQueue = new Queue<MapThreadInfo<float[,]>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private float[,] falloffMap;


    private void Awake()
    {
        MapTexture.regions = regions;
        falloffMap = FalloffGenerator.GenerateFalloffMap(chunkSize);
    }

    void OnValuesChanged ()
    {
        if (!Application.isPlaying)
            DrawMapInEditor();
    }

    public void DrawMapInEditor()
    {
        float[,] noiseMap = GenerateNoiseMap();

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        if (currentDrawMode == DrawMode.NoiseMap)
            mapDisplay.DrawMapFromNosieMap(noiseMap);
        else if (currentDrawMode == DrawMode.TerrainMap)
            mapDisplay.DrawMapFromTexture2D(MapTexture.GenerateTextureFromNoiseMap(noiseMap, regions), new Vector2Int(chunkSize, chunkSize));
        else if (currentDrawMode == DrawMode.Mesh)
            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplierCurve, terrainData.tileSize, editorPreviewlevelOfDetail), 
                MapTexture.GenerateTextureFromNoiseMap(noiseMap, regions), 
                new Vector2Int(chunkSize, chunkSize));
    }

    public void RequestNosieMap(Vector2 center, Action<float[,]> callback)
    {
        ThreadStart threadStart = delegate
        {
            NosieThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void NosieThread(Vector2 center, Action<float[,]> callback)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(new Vector2Int(chunkSize, chunkSize), noiseData.seed, noiseData.noiseScale,
            noiseData.octaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.customOffset);
        lock (noiseThreadInfoQueue)
        {
            noiseThreadInfoQueue.Enqueue(new MapThreadInfo<float[,]>(callback, noiseMap));
        }
    }

    public void RequestMeshData(float[,] nosieMape, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(nosieMape, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(float[,] nosieMape, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(nosieMape, terrainData.meshHeightMultiplierCurve, new Vector2Int(1,1), lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if (noiseThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < noiseThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<float[,]> threadInfo = noiseThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap (new Vector2Int(chunkSize, chunkSize), noiseData.seed, noiseData.noiseScale,
            noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.customOffset);

        if (terrainData.useFalloff)
        {
            for (int i = 0; i < chunkSize; i++)
            {
                for (int j = 0; j < chunkSize; j++)
                {
                    noiseMap[i, j] = Mathf.Clamp(0, 1, noiseMap[i, j] - falloffMap[i, j]) ;
                }
            }
        }

        return noiseMap;
    }

    public void GenerateMapFromTexture()
    {
        if (noiseMapInput == null)
            return;

        float[,] noiseMap = new float[noiseMapInput.width, noiseMapInput.height];
        for (int i = 0; i < noiseMapInput.width; i++)
        {
            for (int j = 0; j < noiseMapInput.height; j++)
            {
                noiseMap[i, j] = noiseMapInput.GetPixel(i, j).grayscale;
            }
        }
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplierCurve, terrainData.tileSize, editorPreviewlevelOfDetail), MapTexture.GenerateTextureFromNoiseMap(noiseMap, regions), new Vector2Int(chunkSize, chunkSize));
    }


    private void OnValidate()
    {
        if (terrainData != null)
        {
            terrainData.OnValuesChanged -= OnValuesChanged;
            terrainData.OnValuesChanged += OnValuesChanged;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesChanged -= OnValuesChanged;
            noiseData.OnValuesChanged += OnValuesChanged;
        }

    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
