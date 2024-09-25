using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pushable))]
public class PushableEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Pushable pushable = target as Pushable;

    }
}
