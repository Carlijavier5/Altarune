using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeMagicCircleController))]
public class TimeMagicCircleControllerEditor : Editor
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
        
        TimeMagicCircleController controller = (TimeMagicCircleController)target;

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            if (GUILayout.Button("Switch Directions"))
            {
                controller.SwitchDirections();
            }
        }

        so.ApplyModifiedProperties();
    }
}
