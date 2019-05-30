using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event System.Action OnValuesChanged;
    public bool autoUpdate;

    public void FireOnValuesChanged()
    {
        if (OnValuesChanged != null)
            OnValuesChanged();
    }

    protected virtual void OnValidate()
    {
        if (autoUpdate)
            FireOnValuesChanged();
    }
}
