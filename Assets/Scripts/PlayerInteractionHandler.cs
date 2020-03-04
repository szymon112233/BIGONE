using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    int interactionLayer = 12;

    [SerializeField]
    List<GenericInteractible> nearbyInteractibles;
    [SerializeField]
    int closestIndex = 0;
    float closesDistanceSquared = float.MaxValue;

    private void Awake()
    {
        nearbyInteractibles = new List<GenericInteractible>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == interactionLayer)
            AddToNearbyList(other.gameObject.transform.parent.GetComponent<GenericInteractible>());                
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == interactionLayer)
            RemoveFromNearbyList(other.transform.parent.GetComponent<GenericInteractible>());
            
    }

    void AddToNearbyList(GenericInteractible interactible)
    {
        if (interactible != null)
            if (!nearbyInteractibles.Contains(interactible))
            {
                nearbyInteractibles.Add(interactible);
                interactible.OnBecameDisabled += RemoveFromNearbyList;
            }             
            else
                Debug.LogWarningFormat("{0}, {1}: Trying to add {2}, but it's already in the list!", gameObject.name, this.GetType().Name, interactible.gameObject.name);
        else
            Debug.LogWarningFormat("{0}, {1}: Could not find GenericInteractible component on: {2}", gameObject.name, this.GetType().Name, interactible.gameObject.name);
    }

    void RemoveFromNearbyList(GenericInteractible interactible)
    {
        if (interactible != null)
            if (nearbyInteractibles.Contains(interactible))
                nearbyInteractibles.Remove(interactible);
            else
                Debug.LogWarningFormat("{0}, {1}: Could not find an item: {2}, while trying to remove it from list", gameObject.name, this.GetType().Name, interactible.gameObject.name);
        else
            Debug.LogWarningFormat("{0}, {1}: Could not find GenericInteractible component on: {2}", gameObject.name, this.GetType().Name, interactible.gameObject.name);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearbyInteractibles.Count != 0)
            nearbyInteractibles[closestIndex].Interact();

        closesDistanceSquared = float.MaxValue;
        for (int i = 0; i < nearbyInteractibles.Count; i++)
        {
            float distanceSquared = (nearbyInteractibles[i].gameObject.transform.position - transform.position).sqrMagnitude;
            if (distanceSquared < closesDistanceSquared)
            {
                closesDistanceSquared = distanceSquared;
                closestIndex = i;
            }

        }

    }
}
