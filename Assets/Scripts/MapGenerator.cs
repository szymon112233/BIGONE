using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, TerrainMap, Mesh};
    public DrawMode currentDrawMode;

    public bool autoUpdate;
    public Vector2Int mapSize;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 customOffset;
    public Vector2Int tileSize;

    public float meshHeightMultiplier;

    public Texture2D noiseMapInput;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, seed, noiseScale, octaves, persistance, lacunarity, customOffset);

        Color[] colourMap = new Color[mapSize.x * mapSize.y];

        for (int j = 0; j < mapSize.y; j++)
        {
            for (int i = 0; i < mapSize.x; i++)
            {
                float currentHeight = noiseMap[i, j];
                for (int r = 0; r < regions.Length; r++)
                {
                    if (currentHeight <= regions[r].height)
                    {
                        colourMap[j * mapSize.x + i] = regions[r].color;
                        break;
                    }
                }

            }
        }

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        if (currentDrawMode == DrawMode.NoiseMap)
            mapDisplay.DrawMapFromNosieMap(noiseMap);
        else if (currentDrawMode == DrawMode.TerrainMap)
            mapDisplay.DrawMapFromColorMap(colourMap, new Vector2Int(mapSize.x, mapSize.y));
        else if (currentDrawMode == DrawMode.Mesh)
            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, tileSize), colourMap, new Vector2Int(mapSize.x, mapSize.y));
    }

    public void GenerateMapFromTexture()
    {
        if (noiseMapInput == null)
            return;
        autoUpdate = false;
        Color[] colors = noiseMapInput.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            float currentHeight = colors[i].grayscale;
            for (int r = 0; r < regions.Length; r++)
            {
                if (currentHeight <= regions[r].height)
                {
                    colors[i] = regions[r].color;
                    break;
                }
            }
        }
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        mapDisplay.DrawMapFromColorMap(colors, new Vector2Int(noiseMapInput.width, noiseMapInput.height));
        mapDisplay.DrawMap();

    }

    private void OnValidate()
    {
        if (mapSize.x < 1)
            mapSize.x = 1;
        if (mapSize.y < 1)
            mapSize.y = 1;

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
