using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDeathCondition : CCondition {
    [SerializeField] private Entity entity;

    private void Start() {
        entity.OnPerish += HandlePerish;
    }

    private void HandlePerish(BaseObject baseObject) {
        CheckCondition();
    }
}
