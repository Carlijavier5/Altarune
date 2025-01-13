using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CCondition")]
public class CConditionData : ScriptableObject {
    public DialogueData dialogueData;
    public CConditionData nextCondition;
    
    //PARAMS
    public bool lockPlayerControls = false;
}
