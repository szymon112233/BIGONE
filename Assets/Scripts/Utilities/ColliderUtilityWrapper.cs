using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderUtilityWrapper : MonoBehaviour
{
    public bool IsColliding()
    {
        return collisionCount != 0;
    }

    [SerializeField]
    int collisionCount = 0;

    private void OnCollisionEnter(Collision collision)
    {
        collisionCount++;
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionCount--;
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        collisionCount--;
    }
}
