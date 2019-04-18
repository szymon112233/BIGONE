using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{

    public Renderer textureRenderer;
    public Texture2D mapTexture;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Transform waterPlaneTransform;
    public TileFinder tileFinder;
    public MeshCollider collider;

    public void DrawMapFromNosieMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Color[] colourMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        DrawMapFromColorMap(colourMap, new Vector2Int(width, height));
    }

    public void DrawMapFromColorMap(Color[] colorMap, Vector2Int textureSize)
    {
        mapTexture = new Texture2D(textureSize.x, textureSize.y);
        mapTexture.SetPixels(colorMap);
        mapTexture.filterMode = FilterMode.Point;
        mapTexture.wrapMode = TextureWrapMode.Clamp;
        mapTexture.Apply();
        DrawMap();
    }
    public void DrawMesh(MeshData mesh, Color[] colorMap, Vector2Int textureSize)
    {
        tileFinder.meshData = mesh;
        meshFilter.sharedMesh = mesh.CreateMesh();

        collider.sharedMesh = null;
        collider.sharedMesh = meshFilter.sharedMesh;

        mapTexture = new Texture2D(textureSize.x, textureSize.y);
        mapTexture.SetPixels(colorMap);
        mapTexture.filterMode = FilterMode.Point;
        mapTexture.wrapMode = TextureWrapMode.Clamp;
        mapTexture.Apply();

        meshRenderer.sharedMaterial.mainTexture = mapTexture;

        MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        float waterLevel = mapGen.regions[0].height * mapGen.meshHeightMultiplier;
        waterPlaneTransform.position = new Vector3(0, waterLevel, 0);  
    }


    public void DrawMap()
    {
        textureRenderer.sharedMaterial.mainTexture = mapTexture;
        textureRenderer.transform.localScale = new Vector3(mapTexture.width, 1, mapTexture.height);
    }
}
