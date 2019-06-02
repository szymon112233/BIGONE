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
            return ElementsNearby.Count > 0;
        }

    }

    public List<StaticBuildingElement> ElementsNearby;
    int collisionsCount = 0;
    int currentElementPrefab = 0;
    bool isBuilding = false;
    bool buildable;
    int buildingsPlaced = 0;
    

    private void Awake()
    {
        buildingElementPreviewMeshFilter = buildingElementPreview.GetComponent<MeshFilter>();
        buildingElementPreviewMeshFilter.mesh = bulidingElementsPrefabs[currentElementPrefab].GetComponent<MeshFilter>().sharedMesh;
        buildingElementPreview.transform.localScale = bulidingElementsPrefabs[currentElementPrefab].transform.localScale;
        buildingElementPreviewMeshRenderer = buildingElementPreview.GetComponent<MeshRenderer>();
        ElementsNearby = new List<StaticBuildingElement>();
    }

    private void FixedUpdate()
    {


        if (isOtherBuildingPartNearby)
        {
            StaticBuildingElement closestElement = ElementsNearby[0];
            Debug.Log(closestElement.transform.InverseTransformPoint(transform.position));
            float closestElementDistance = closestElement.bounds.SqrDistance(closestElement.transform.InverseTransformPoint(transform.position) * closestElement.transform.localScale.x);
            for (int i = 1; i < ElementsNearby.Count; i++)
            {
                //closestElement.transform.InverseTransformPoint(transform.position)
                float distance = ElementsNearby[i].bounds.SqrDistance(ElementsNearby[i].transform.InverseTransformPoint(transform.position) * closestElement.transform.localScale.x);
                if (closestElementDistance >= distance)
                {
                    closestElementDistance = distance;
                    closestElement = ElementsNearby[i];
                }
            }
            Debug.Log(closestElement.gameObject.name);
            Vector3 closestPoint = closestElement.bounds.ClosestPoint(closestElement.transform.InverseTransformPoint(transform.position) * closestElement.transform.localScale.x);
            Debug.Log(closestElementDistance.ToString());
            Vector3 snapPoint = new Vector3();
            if (closestPoint.x > 0 && closestPoint.x >  Mathf.Abs(closestPoint.z))
            {
                snapPoint = closestElement.position + new Vector3(closestElement.bounds.extents.x * 2, 0, 0); 
            }
            else if (closestPoint.x < 0 && -closestPoint.x > Mathf.Abs(closestPoint.z))
            {
                snapPoint = closestElement.position - new Vector3(closestElement.bounds.extents.x * 2, 0, 0);
            }
            else if (closestPoint.z > 0 && closestPoint.z > Mathf.Abs(closestPoint.x))
            {
                snapPoint = closestElement.position + new Vector3(0, 0, closestElement.bounds.extents.z * 2);
            }
            else if (closestPoint.z < 0 && -closestPoint.z > Mathf.Abs(closestPoint.x))
            {
                snapPoint = closestElement.position - new Vector3(0, 0, closestElement.bounds.extents.z * 2);
            }

            //Watch out for the floats' precision
            buildable = true;
            for (int i = 0; i < ElementsNearby.Count; i++)
            {
                if (snapPoint == ElementsNearby[i].position)
                {
                    buildable = false;
                    break;
                }
            }
            if (buildable)
                buildingElementPreview.transform.position = snapPoint;
            else
            {
                buildingElementPreview.transform.position = Vector3.zero;
            }
        }
        else
        {
            buildingElementPreview.transform.position = transform.position;
            buildable = true;
        }


        if (buildable)
        {
            buildingElementPreviewMeshRenderer.material.SetInt("_isBuildable", 1);
        }
        else
        {
            buildingElementPreviewMeshRenderer.material.SetInt("_isBuildable", 0);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Place") && buildable)
        {
            GameObject go =  Instantiate(bulidingElementsPrefabs[currentElementPrefab], buildingElementPreview.transform.position, Quaternion.identity);
            go.name = string.Format("{0}_{1}", go.name, buildingsPlaced);
            buildingsPlaced++;
            StaticBuildingElement element = go.GetComponent<StaticBuildingElement>();
            element.position = buildingElementPreview.transform.position;
            element.bounds = bulidingElementsPrefabs[currentElementPrefab].GetComponent<MeshFilter>().sharedMesh.bounds;
            element.bounds.extents *= go.transform.localScale.x;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        ElementsNearby.Add(other.GetComponent<StaticBuildingElement>());
        collisionsCount++;
    }
    private void OnTriggerExit(Collider other)
    {

        ElementsNearby.Remove(other.GetComponent<StaticBuildingElement>());
        collisionsCount--;
    }

}
