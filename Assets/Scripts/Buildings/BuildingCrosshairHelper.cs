using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCrosshairHelper : MonoBehaviour
{
    public Material buildableMaterial;
    public Material unbuildableMaterial;
    public GameObject[] bulidingElementsPrefabs;

    public GameObject buildingElementPreview;

    MeshFilter buildingElementPreviewMeshFilter;
    MeshRenderer buildingElementPreviewMeshRenderer;


    bool isOtherBuildingPartNearby
    {
       get
        {
            return collisionsCount > 0;
        }

    }

    int collisionsCount = 0;
    int currentElementPrefab = 0;
    bool isBuilding = false;
    

    private void Awake()
    {
        buildingElementPreviewMeshFilter = buildingElementPreview.GetComponent<MeshFilter>();
        buildingElementPreviewMeshFilter.mesh = bulidingElementsPrefabs[currentElementPrefab].GetComponent<MeshFilter>().sharedMesh;
        buildingElementPreview.transform.localScale = bulidingElementsPrefabs[currentElementPrefab].transform.localScale;
        buildingElementPreviewMeshRenderer = buildingElementPreview.GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {


        if (isOtherBuildingPartNearby)
        {
            buildingElementPreviewMeshRenderer.material = unbuildableMaterial;
        }
        else
        {
            buildingElementPreview.transform.position = transform.position;
            buildingElementPreviewMeshRenderer.material = buildableMaterial;
        }

        
    }

    private void Update()
    {
        if (Input.GetButtonDown("Place"))
        {
            Instantiate(bulidingElementsPrefabs[currentElementPrefab], transform.position, Quaternion.identity);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("Enter: {0}", other.gameObject.name);
        collisionsCount++;
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        collisionsCount--;
    }

}
