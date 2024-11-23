using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ManaShieldController))]
public class ManaShieldControllerEditor : Editor 
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
        
        ManaShieldController controller = (ManaShieldController)target;

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
           if (GUILayout.Button("Simulate Spawn"))
            {
                controller.ShieldSpawn();
            }
        
            if (GUILayout.Button("Simulate Hit"))
            {
                controller.SimulateHit();
            }
            
            if (GUILayout.Button("Simulate Break"))
            {
                controller.ShieldBreak();
            }
            
            if (GUILayout.Button("Reset")) {
                controller.ResetMaterialParams();
            }
        }

        so.ApplyModifiedProperties();
    }
}
