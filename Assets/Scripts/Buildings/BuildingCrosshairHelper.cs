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
        buildingElementPreviewMeshRenderer = buildingElementPreview.GetComponent<MeshRenderer>();
        ElementsNearby = new List<StaticBuildingElement>();

        buildingElementPreviewMeshFilter.mesh = bulidingElementsPrefabs[currentElementPrefab].GetComponent<MeshFilter>().sharedMesh;
        buildingElementPreview.transform.localScale = bulidingElementsPrefabs[currentElementPrefab].transform.localScale;
    }

    private void FixedUpdate()
    {
        if (isOtherBuildingPartNearby)
        {
            StaticBuildingElement closestElement = ElementsNearby[0];
            //Debug.Log(closestElement.transform.InverseTransformPoint(transform.position));
            float closestElementDistance = closestElement.bounds.SqrDistance(closestElement.transform.InverseTransformPoint(transform.position) * (int)closestElement.size);
            for (int i = 1; i < ElementsNearby.Count; i++)
            {
                //closestElement.transform.InverseTransformPoint(transform.position)
                float distance = ElementsNearby[i].bounds.SqrDistance(ElementsNearby[i].transform.InverseTransformPoint(transform.position) * (int)closestElement.size);
                if (closestElementDistance >= distance)
                {
                    closestElementDistance = distance;
                    closestElement = ElementsNearby[i];
                }
            }
            //Debug.Log(closestElement.gameObject.name);
            Vector3 closestPoint = closestElement.bounds.ClosestPoint(closestElement.transform.InverseTransformPoint(transform.position) * (int)closestElement.size);
            //Debug.Log(closestElementDistance.ToString());
            Vector3 snapPoint = new Vector3();
            int buildElementSize = (int)bulidingElementsPrefabs[currentElementPrefab].GetComponent<StaticBuildingElement>().size;
            if (closestPoint.x > 0 && closestPoint.x >  Mathf.Abs(closestPoint.z))
            {
                //Set position to the center of closest element
                snapPoint = closestElement.position;
                //Move it to the edge
                snapPoint += new Vector3((int)closestElement.size/2, 0, 0);
                //Move it so that they only share one wall
                snapPoint += new Vector3(buildElementSize/2, 0, 0);
                
                //Calculate which "slot" is closest       
                float closestPointPositionZ = closestPoint.z;
                closestPointPositionZ += (int)closestElement.size / 2;
                float divideRatio = 1.0f / (buildElementSize / 2 + (int)closestElement.size / 2 - 1);
                int snapSlot = (int)((closestPointPositionZ / (int)closestElement.size) / divideRatio);
                float finalMovePositionZ = -buildElementSize / 2 + (int)ElementSize.Small + (int)ElementSize.Small * snapSlot;
                finalMovePositionZ -= (int)closestElement.size / 2;

                snapPoint += new Vector3(0, 0, finalMovePositionZ);
            }
            else if (closestPoint.x < 0 && -closestPoint.x > Mathf.Abs(closestPoint.z))
            {
                //Set position to the center of closest element
                snapPoint = closestElement.position;
                //Move it to the edge
                snapPoint -= new Vector3((int)closestElement.size / 2, 0, 0);
                //Move it so that they only share one wall
                snapPoint -= new Vector3(buildElementSize / 2, 0, 0);

                //Calculate which "slot" is closest       
                float closestPointPositionZ = closestPoint.z;
                closestPointPositionZ += (int)closestElement.size / 2;
                float divideRatio = 1.0f / (buildElementSize / 2 + (int)closestElement.size / 2 - 1);
                int snapSlot = (int)((closestPointPositionZ / (int)closestElement.size) / divideRatio);
                float finalMovePositionZ = -buildElementSize / 2 + (int)ElementSize.Small + (int)ElementSize.Small * snapSlot;
                finalMovePositionZ -= (int)closestElement.size / 2;

                snapPoint += new Vector3(0, 0, finalMovePositionZ);
            }
            else if (closestPoint.z > 0 && closestPoint.z > Mathf.Abs(closestPoint.x))
            {
                //Set position to the center of closest element
                snapPoint = closestElement.position;
                //Move it to the edge
                snapPoint += new Vector3(0, 0, (int)closestElement.size / 2);
                //Move it so that they only share one wall
                snapPoint += new Vector3(0, 0, buildElementSize / 2);

                //Calculate which "slot" is closest       
                float closestPointPositionX = closestPoint.x;
                closestPointPositionX += (int)closestElement.size / 2;
                float divideRatio = 1.0f / (buildElementSize / 2 + (int)closestElement.size / 2 - 1);
                int snapSlot = (int)((closestPointPositionX / (int)closestElement.size) / divideRatio);
                float finalMovePositionX = -buildElementSize / 2 + (int)ElementSize.Small + (int)ElementSize.Small * snapSlot;
                finalMovePositionX -= (int)closestElement.size / 2;

                snapPoint += new Vector3(finalMovePositionX, 0, 0);
            }
            else if (closestPoint.z < 0 && -closestPoint.z > Mathf.Abs(closestPoint.x))
            {
                //Set position to the center of closest element
                snapPoint = closestElement.position;
                //Move it to the edge
                snapPoint -= new Vector3(0, 0, (int)closestElement.size / 2);
                //Move it so that they only share one wall
                snapPoint -= new Vector3(0, 0, buildElementSize / 2);

                //Calculate which "slot" is closest       
                float closestPointPositionX = closestPoint.x;
                closestPointPositionX += (int)closestElement.size / 2;
                float divideRatio = 1.0f / (buildElementSize / 2 + (int)closestElement.size / 2 - 1);
                int snapSlot = (int)((closestPointPositionX / (int)closestElement.size) / divideRatio);
                float finalMovePositionX = -buildElementSize / 2 + (int)ElementSize.Small + (int)ElementSize.Small * snapSlot;
                finalMovePositionX -= (int)closestElement.size / 2;

                snapPoint += new Vector3(finalMovePositionX, 0, 0);
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

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentElementPrefab++;
            if (currentElementPrefab >= bulidingElementsPrefabs.Length)
                currentElementPrefab = bulidingElementsPrefabs.Length - 1;
            buildingElementPreviewMeshFilter.mesh = bulidingElementsPrefabs[currentElementPrefab].GetComponent<MeshFilter>().sharedMesh;
            buildingElementPreviewMeshRenderer.material.mainTexture = bulidingElementsPrefabs[currentElementPrefab].GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            buildingElementPreview.transform.localScale = bulidingElementsPrefabs[currentElementPrefab].transform.localScale;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentElementPrefab--;
            if (currentElementPrefab < 0)
                currentElementPrefab = 0;
            buildingElementPreviewMeshFilter.mesh = bulidingElementsPrefabs[currentElementPrefab].GetComponent<MeshFilter>().sharedMesh;
            buildingElementPreviewMeshRenderer.material.mainTexture = bulidingElementsPrefabs[currentElementPrefab].GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            buildingElementPreview.transform.localScale = bulidingElementsPrefabs[currentElementPrefab].transform.localScale;
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
