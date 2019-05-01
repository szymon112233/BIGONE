using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(Vector2Int mapSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 customOffset)
    {
        float[,] noiseMap = new float[mapSize.x, mapSize.y];
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];


        float amplitude = 1;
        float frequency = 1;
        float maxPossibleHeight = 0f;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + customOffset.x;
            float offsetY = prng.Next(-100000, 100000) - customOffset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
            scale = 0.00001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseValue = float.MaxValue;

        float halfWidth = mapSize.x / 2f;
        float halfHeight = mapSize.y / 2f;

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                
                for (int z = 0; z < octaves; z++)
                {


                    float sampleX = (i - halfWidth + octaveOffset[z].x) / scale * frequency;
                    float sampleY = (j - halfHeight + octaveOffset[z].y) / scale * frequency;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                    
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseValue)
                    minNoiseValue = noiseHeight;

                noiseMap[i, j] = noiseHeight;

            }
        }

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                float normalizedHeight = noiseMap[i, j] + 1 / (2f * maxPossibleHeight / 1.75f);

                noiseMap[i, j] = Mathf.Clamp(normalizedHeight,0, int.MaxValue);
            }
        }


        return noiseMap;
    }
}
