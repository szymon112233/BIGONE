using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A generic pickup that throws an event when picked=up and then it destroys itself
public class GenericPickup : GenericInteractible
{
    public override void Awake()
    {
        base.Awake();
        interactibleType = InteractibleType.PICKUP;
    }


    public override void Interact()
    {
        base.Interact();
        Destroy(gameObject);
    }
}
