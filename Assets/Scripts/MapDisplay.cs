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

    public void DrawMapFromTexture2D(Texture2D texture, Vector2Int textureSize)
    {
        mapTexture = texture;
        DrawMap();
    }

    public void DrawMapFromColorMap(Color[] colourMap, Vector2Int textureSize)
    {
        mapTexture = MapTexture.GenerateTextureFromColorMap(colourMap, textureSize);
        DrawMap();
    }

    public void DrawMesh(MeshData mesh, Texture2D texture, Vector2Int textureSize)
    {
        meshFilter.sharedMesh = mesh.CreateMesh();

        collider.sharedMesh = null;
        collider.sharedMesh = meshFilter.sharedMesh;

        mapTexture = texture;

        meshRenderer.sharedMaterial.mainTexture = mapTexture;

        MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        float waterLevel = mapGen.regions[0].height * mapGen.meshHeightMultiplierCurve.Evaluate(mapGen.regions[0].height);
        waterPlaneTransform.position = new Vector3(0, waterLevel, 0);
    }

    public void RedrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();

        collider.sharedMesh = null;
        collider.sharedMesh = meshFilter.sharedMesh;
    }

    public void DrawMap()
    {
        textureRenderer.sharedMaterial.mainTexture = mapTexture;
        textureRenderer.transform.localScale = new Vector3(mapTexture.width, 1, mapTexture.height);
    }
}
