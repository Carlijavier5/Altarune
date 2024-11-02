using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager component for player skills
/// Copies a lot from SummonController
/// </summary>
public class PlayerSkillController : MonoBehaviour
{
    private PlayerController _inputSource;

    private PlayerSkillData _activeSkill;

    [SerializeField] private PlayerSkillData _skillBlueprints;

    void Awake() {
        _inputSource.OnPlayerInit += InputSource_OnPlayerInit;
    }

    private void InputSource_OnPlayerInit() {
        _inputSource.OnSkillPerformed += PerformSkill;
    }

    private void PerformSkill() {
        // code here for declining the skill use if you don't have enough energ
    }
}
