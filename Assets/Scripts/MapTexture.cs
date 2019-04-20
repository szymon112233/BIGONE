﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapTexture
{
    public static Texture2D GenerateTextureFromNoiseMap(float[,] noiseMap, TerrainType[] regions)
    {
        Vector2Int mapSize = new Vector2Int(noiseMap.GetLength(0), noiseMap.GetLength(1));

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

        Texture2D texture = new Texture2D(mapSize.x, mapSize.y);
        texture.SetPixels(colourMap);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        return texture;
    }

    public static Texture2D GenerateTextureFromColorMap(Color[] colorMap, Vector2Int mapSize)
    {
        Texture2D texture = new Texture2D(mapSize.x, mapSize.y);
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        return texture;
    }
}