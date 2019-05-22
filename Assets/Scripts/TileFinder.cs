using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFinder : MonoBehaviour
{
    public EndlessTerrain endlessTerrain;
    public LineRenderer lineRenderer;
    public int brushSize;

    ClickableTile currentTile;
    Vector2Int lastPos;

    void Start()
    {
        lastPos = new Vector2Int();
    }

    private void FixedUpdate()
    {
        FindAndSelectTiles();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.KeypadPlus))
        //{
        //    Vector3 prevPos = meshData.vertices[currentTile.topLeft];
        //    meshData.vertices[currentTile.topLeft] = new Vector3(prevPos.x, prevPos.y + heightChangeStep, prevPos.z);

        //    prevPos = meshData.vertices[currentTile.topRight];
        //    meshData.vertices[currentTile.topRight] = new Vector3(prevPos.x, prevPos.y + heightChangeStep, prevPos.z);

        //    prevPos = meshData.vertices[currentTile.bottomLeft];
        //    meshData.vertices[currentTile.bottomLeft] = new Vector3(prevPos.x, prevPos.y + heightChangeStep, prevPos.z);

        //    prevPos = meshData.vertices[currentTile.bottomRight];
        //    meshData.vertices[currentTile.bottomRight] = new Vector3(prevPos.x, prevPos.y + heightChangeStep, prevPos.z);

        //    MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        //    mapDisplay.RedrawMesh(meshData);
        //}
        //else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        //{
        //    Vector3 prevPos = meshData.vertices[currentTile.topLeft];
        //    meshData.vertices[currentTile.topLeft] = new Vector3(prevPos.x, prevPos.y - heightChangeStep, prevPos.z);

        //    prevPos = meshData.vertices[currentTile.topRight];
        //    meshData.vertices[currentTile.topRight] = new Vector3(prevPos.x, prevPos.y - heightChangeStep, prevPos.z);

        //    prevPos = meshData.vertices[currentTile.bottomLeft];
        //    meshData.vertices[currentTile.bottomLeft] = new Vector3(prevPos.x, prevPos.y - heightChangeStep, prevPos.z);

        //    prevPos = meshData.vertices[currentTile.bottomRight];
        //    meshData.vertices[currentTile.bottomRight] = new Vector3(prevPos.x, prevPos.y - heightChangeStep, prevPos.z);

        //    MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        //    mapDisplay.RedrawMesh(meshData);
        //}
    }

    void FindAndSelectTiles()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 9; // Check only for terrain
        if (endlessTerrain != null && Physics.Raycast(ray, out hit, 10000, layerMask))
        {
            int chunkSize = MapGenerator.chunkSize - 1;

            //Manual calculation
            //Vector2 chunkCoords = new Vector2(Mathf.RoundToInt(hit.point.x/ chunkSize), Mathf.RoundToInt(hit.point.z/ chunkSize));

            Vector3 reference_point = hit.collider.gameObject.transform.position;
            Vector2 chunkCoords = new Vector2((int)reference_point.x / chunkSize, (int)reference_point.z / chunkSize);
            //Debug.LogFormat("Chunk: [{0}|{1}] ", chunkCoords.x, chunkCoords.y);
            MeshData meshData = endlessTerrain.terrainChunkDictionary[chunkCoords].meshDataForTileFinder;
            if (meshData == null)
                return;
            transform.position = hit.point;

            Vector2Int pos = new Vector2Int(); // o tu corner case co jeźeli point.x = -0.11 to samo co x = 0.11
            pos.x = (int)hit.point.x - (int)chunkCoords.x * chunkSize;
            pos.y = (int)hit.point.z - (int)chunkCoords.y * chunkSize;

            //Corner case fix
            if (hit.point.x < 0)
                pos.x--;
            if (hit.point.z > 0)
                pos.y++;


            //Debug.LogFormat("Looking for pos: [{0}, {1}], hit.point.x = {2}, meshData.tileSize.x = {3}", pos.x, pos.y, hit.point.x, meshData.tileSize.x);
            //Debug.LogFormat("meshData.tiles.ContainsKey(pos): {0}", meshData.tiles.ContainsKey(pos));
            if (lastPos != pos && meshData.tiles.ContainsKey(pos))
            {
                lastPos = pos;
                currentTile = meshData.tiles[pos]; // Currently no purpose since mesh data changes
                Vector3 additionalheight = new Vector3(0, 0.1f, 0);


                lineRenderer.positionCount = 5;
                Vector3[] positions = new Vector3[5];
                positions[0] = reference_point + meshData.vertices[meshData.tiles[pos].topLeft] + additionalheight;
                //Debug.LogFormat("positions[0]: {0}", positions[0]);

                positions[1] = reference_point + meshData.vertices[meshData.tiles[pos].topRight] + additionalheight;
                //Debug.LogFormat("positions[1]: {0}", positions[1]);

                positions[2] = reference_point + meshData.vertices[meshData.tiles[pos].bottomRight] + additionalheight;
                //Debug.LogFormat("positions[2]: {0}", positions[2]);

                positions[3] = reference_point + meshData.vertices[meshData.tiles[pos].bottomLeft] + additionalheight;
                //Debug.LogFormat("positions[3]: {0}", positions[3]);

                positions[4] = reference_point + meshData.vertices[meshData.tiles[pos].topLeft] + additionalheight;

                lineRenderer.SetPositions(positions);
            }
        }
    }
}
