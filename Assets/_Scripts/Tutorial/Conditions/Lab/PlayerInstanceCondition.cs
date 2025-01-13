using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstanceCondition : CCondition
{
    private void Start() {
        SetActive(true);
        GM.Instance.OnPlayerInit += CheckCondition;
    }
}
