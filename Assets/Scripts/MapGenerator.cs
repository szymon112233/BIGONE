using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, TerrainMap, Mesh};
    public DrawMode currentDrawMode;

    public const int chunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;
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

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap (new Vector2Int(chunkSize, chunkSize), seed, noiseScale, octaves, persistance, lacunarity, customOffset);

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        if (currentDrawMode == DrawMode.NoiseMap)
            mapDisplay.DrawMapFromNosieMap(noiseMap);
        else if (currentDrawMode == DrawMode.TerrainMap)
            mapDisplay.DrawMapFromTexture2D(MapTexture.GenerateTextureFromNoiseMap(noiseMap, regions), new Vector2Int(chunkSize, chunkSize));
        else if (currentDrawMode == DrawMode.Mesh)
            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplierCurve, tileSize, levelOfDetail), MapTexture.GenerateTextureFromNoiseMap(noiseMap, regions), new Vector2Int(chunkSize, chunkSize));
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
        mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplierCurve, tileSize, levelOfDetail), MapTexture.GenerateTextureFromNoiseMap(noiseMap, regions), new Vector2Int(chunkSize, chunkSize));
    }

    private void OnValidate()
    {

        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
