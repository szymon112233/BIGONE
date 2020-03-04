using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GenericInventory))]
public class InventoryGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //EditorGUILayout.HelpBox("This is a help box", MessageType.Info);
        GenericInventory myScript = (GenericInventory)target;
        if (GUILayout.Button("Spawn random item"))
        {
            myScript.AddRandomItem();
        }
    }
}