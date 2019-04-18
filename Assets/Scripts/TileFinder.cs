using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFinder : MonoBehaviour
{
    public MeshData meshData;
    public LineRenderer lineRenderer;

    Vector2Int lastPos;

    void Start()
    {
        lastPos = new Vector2Int();
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (meshData != null && Physics.Raycast(ray, out hit, 10000))
        {
            transform.position = hit.point;
            Vector2Int pos = new Vector2Int((int)hit.point.x/ meshData.tileSize.x, (int)hit.point.z/ meshData.tileSize.y);
            Debug.LogFormat("Looking for pos: [{0}, {1}]", pos.x, pos.y);
            if (lastPos != pos && meshData.tiles.ContainsKey(pos))
            {
                lastPos = pos;
                Vector3 additionalheight = new Vector3(0, 1, 0);
                lineRenderer.positionCount = 5;
                lineRenderer.SetPosition(0, meshData.vertices[meshData.tiles[pos].topLeft] + additionalheight);
                lineRenderer.SetPosition(1, meshData.vertices[meshData.tiles[pos].topRight] + additionalheight);
                lineRenderer.SetPosition(2, meshData.vertices[meshData.tiles[pos].bottomRight] + additionalheight);
                lineRenderer.SetPosition(3, meshData.vertices[meshData.tiles[pos].bottomLeft] + additionalheight);
                lineRenderer.SetPosition(4, meshData.vertices[meshData.tiles[pos].topLeft] + additionalheight);
            }
        }
    }
}
