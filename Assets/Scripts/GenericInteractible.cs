using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractibleType
{
    INTERACTIBLE = 0,
    PICKUP,
    COUNT
}

//A generic Interactible that throws an event when picked=up and then it destroys itself
public class GenericInteractible : MonoBehaviour
{
    //references
    public ColliderUtilityWrapper InteractionColider;
    public GameObject LabelGO;
    public TMPro.TextMeshPro labelText;

    public InteractibleType interactibleType = InteractibleType.INTERACTIBLE;
    public string Name
    {
        get
        {
            return name;
        }
         set
        {
            name = value;
            labelText.SetText(value);

        }

    }
    private string name;

    public System.Action OnInteract;
    public System.Action<GenericInteractible> OnBecameDisabled;

    public virtual void Awake()
    {
        InteractionColider.OnCollideStart += ShowLabel;
        InteractionColider.OnCollideEnd += HideLabel;
        Name = gameObject.name;
        HideLabel();
    }

    void ShowLabel()
    {
        LabelGO.SetActive(true);
    }

    void HideLabel()
    {
        LabelGO.SetActive(false);
    }

    public virtual void Interact()
    {
        Debug.LogFormat("Interacting with {0}", gameObject.name);
        if (OnInteract != null)
            OnInteract();
    }

    private void OnDestroy()
    {
        if (OnBecameDisabled != null)
            OnBecameDisabled(this);
    }

    private void OnDisable()
    {
        if (OnBecameDisabled != null)
            OnBecameDisabled(this);
    }
}
