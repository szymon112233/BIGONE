using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{

    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, Vector2Int tileSize)
    {
        if (tileSize.x < 1)
            tileSize.x = 1;
        if (tileSize.y < 1)
            tileSize.y = 1;

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width * tileSize.x - 1) / -2f;
        float topLeftZ = (height * tileSize.y - 1) / 2f;

        MeshData mesh = new MeshData(new Vector2Int(width, height));
        mesh.tileSize = tileSize;
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                mesh.vertices[vertexIndex] = new Vector3(topLeftX + x * tileSize.x, heightMap[x, y] * heightMultiplier, topLeftZ - y* tileSize.y);
                mesh.uvs[vertexIndex] = new Vector2(x/(float)width, y/(float)height);

                if (x < width -1 && y < height -1)
                {
                    mesh.AddTriangle(new Vector3Int(vertexIndex, vertexIndex + width +1, vertexIndex + width));
                    mesh.AddTriangle(new Vector3Int(vertexIndex + width +1, vertexIndex, vertexIndex + 1));

                    Vector2Int pos = new Vector2Int((width - 1) / -2 + x, (height - 1) / 2 - y);
                    ClickableTile tile;
                    tile.topLeft = vertexIndex;
                    tile.topRight = vertexIndex +1;
                    tile.bottomLeft = vertexIndex + width;
                    tile.bottomRight = vertexIndex + width + 1;

                    mesh.tiles.Add(pos, tile);
                    //Debug.LogFormat("Adding element: [{0}|{1}], [{2}, {3}, {4}, {5}]", pos.x, pos.y, tile.topLeft, tile.topRight, tile.bottomLeft, tile.bottomRight);
                }

                vertexIndex++;
            }

        }

        return mesh;
    }

}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Dictionary<Vector2Int, ClickableTile>  tiles;
    public Vector2Int tileSize;

    int triangleIndex;

    public MeshData(Vector2Int meshSize)
    {
        this.vertices = new Vector3[meshSize.x * meshSize.y];
        this.triangles = new int[((meshSize.x -1) * (meshSize.y -1)) * 6];
        this.uvs = new Vector2[meshSize.x * meshSize.y];
        this.tiles = new Dictionary<Vector2Int, ClickableTile>();
    }

    public void AddTriangle(Vector3Int indices)
    {
        triangles[triangleIndex] = indices.x;
        triangles[triangleIndex +1] = indices.y;
        triangles[triangleIndex + 2] = indices.z;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();

        return mesh;
    }
}

public struct ClickableTile
{
    public int topLeft;
    public int topRight;
    public int bottomRight;
    public int bottomLeft;
}