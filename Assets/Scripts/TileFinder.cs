using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFinder : MonoBehaviour
{
    public MeshData meshData;
    public LineRenderer lineRenderer;
    public int heightChangeStep;


    ClickableTile currentTile;

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
            if (hit.point.x > 0)
                pos.x++;
            if (hit.point.z < 0)
                pos.y--;
            Debug.LogFormat("Looking for pos: [{0}, {1}], hit.point.x = {2}, meshData.tileSize.x = {3}", pos.x, pos.y, hit.point.x, meshData.tileSize.x);
            if (lastPos != pos && meshData.tiles.ContainsKey(pos))
            {
                lastPos = pos;
                currentTile = meshData.tiles[pos];
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Vector3 prevPos = meshData.vertices[currentTile.topLeft];
            meshData.vertices[currentTile.topLeft] = new Vector3(prevPos.x, prevPos.y + heightChangeStep, prevPos.z);

            prevPos = meshData.vertices[currentTile.topRight];
            meshData.vertices[currentTile.topRight] = new Vector3(prevPos.x, prevPos.y + heightChangeStep, prevPos.z);

            prevPos = meshData.vertices[currentTile.bottomLeft];
            meshData.vertices[currentTile.bottomLeft] = new Vector3(prevPos.x, prevPos.y + heightChangeStep, prevPos.z);

            prevPos = meshData.vertices[currentTile.bottomRight];
            meshData.vertices[currentTile.bottomRight] = new Vector3(prevPos.x, prevPos.y + heightChangeStep, prevPos.z);

            MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
            mapDisplay.RedrawMesh(meshData);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Vector3 prevPos = meshData.vertices[currentTile.topLeft];
            meshData.vertices[currentTile.topLeft] = new Vector3(prevPos.x, prevPos.y - heightChangeStep, prevPos.z);

            prevPos = meshData.vertices[currentTile.topRight];
            meshData.vertices[currentTile.topRight] = new Vector3(prevPos.x, prevPos.y - heightChangeStep, prevPos.z);

            prevPos = meshData.vertices[currentTile.bottomLeft];
            meshData.vertices[currentTile.bottomLeft] = new Vector3(prevPos.x, prevPos.y - heightChangeStep, prevPos.z);

            prevPos = meshData.vertices[currentTile.bottomRight];
            meshData.vertices[currentTile.bottomRight] = new Vector3(prevPos.x, prevPos.y - heightChangeStep, prevPos.z);

            MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
            mapDisplay.RedrawMesh(meshData);
        }
    }
}
