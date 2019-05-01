﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, TerrainMap, Mesh};
    public DrawMode currentDrawMode;

    public const int chunkSize = 241;
    [Range(0, 6)]
    public int editorPreviewlevelOfDetail;
    public bool autoUpdate;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 customOffset;
    public Vector2Int tileSize;

    public AnimationCurve meshHeightMultiplierCurve;

    public Texture2D noiseMapInput;

    public TerrainType[] regions;

    Queue<MapThreadInfo<float[,]>> noiseThreadInfoQueue = new Queue<MapThreadInfo<float[,]>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();


    private void Awake()
    {

        MapTexture.regions = regions;
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
            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplierCurve, tileSize, editorPreviewlevelOfDetail), 
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
        float[,] noiseMap = Noise.GenerateNoiseMap(new Vector2Int(chunkSize, chunkSize), seed, noiseScale, octaves, persistance, lacunarity, center + customOffset);
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
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(nosieMape, meshHeightMultiplierCurve, new Vector2Int(1,1), lod);
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
        float[,] noiseMap = Noise.GenerateNoiseMap (new Vector2Int(chunkSize, chunkSize), seed, noiseScale, octaves, persistance, lacunarity, customOffset);

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
        mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplierCurve, tileSize, editorPreviewlevelOfDetail), MapTexture.GenerateTextureFromNoiseMap(noiseMap, regions), new Vector2Int(chunkSize, chunkSize));
    }

    private void OnValidate()
    {

        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
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
