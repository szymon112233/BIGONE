using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderUtilityWrapper : MonoBehaviour
{
    public System.Action OnCollideStart;
    public System.Action OnCollideEnd;

    public LayerMask layerMask;

    public bool IsColliding()
    {
        return collisionCount != 0;
    }

    [SerializeField]
    int collisionCount = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (layerMask != (layerMask | 1 << collision.gameObject.layer))
            return;

        if (collisionCount == 0 && OnCollideStart != null)
            OnCollideStart();
        collisionCount++;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (layerMask != (layerMask | 1 << collision.gameObject.layer))
            return;

        collisionCount--;
        if (collisionCount == 0 && OnCollideEnd != null)
            OnCollideEnd();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (layerMask != (layerMask | 1 << other.gameObject.layer))
            return;

        if (collisionCount == 0 && OnCollideStart != null)
            OnCollideStart();
        collisionCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (layerMask != (layerMask | 1 << other.gameObject.layer))
            return;

        collisionCount--;
        if (collisionCount == 0 && OnCollideEnd != null)
            OnCollideEnd();
    }

    private void OnDestroy()
    {
        if (OnCollideEnd != null)
            OnCollideEnd();
    }
}
