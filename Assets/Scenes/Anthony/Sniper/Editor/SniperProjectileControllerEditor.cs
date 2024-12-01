using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SniperProjectileController))]
public class SniperProjectileControllerEditor : Editor
{
    SerializedObject so;
    
    void OnEnable()
    {
        so = serializedObject;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        so.Update();
        
        SniperProjectileController controller = (SniperProjectileController)target;

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            if (GUILayout.Button("Fire"))
            {
                controller.Fire();
            }
        }

        so.ApplyModifiedProperties();
    }
}
