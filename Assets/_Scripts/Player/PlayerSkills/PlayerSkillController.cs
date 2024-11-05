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

    [SerializeField] private PlayerSkillData[] _skillBlueprints;

    private PlayerSkillData _currSkill;

    void Awake() {
        _inputSource = GetComponentInParent<PlayerController>();
        _inputSource.OnPlayerInit += InputSource_OnPlayerInit;
    }

    void Start() { GetRandomSkill(); }

    private void InputSource_OnPlayerInit() {
        _inputSource.OnSkillPerformed += PerformSkill;
    }

    private void PerformSkill() {
        // code here for declining the skill use if you don't have enough energ

        // summon the prefab in direction of the mouse
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        Vector3 targetPos;
        // check if the skill collides with something else (idk if we want to keep this)
        if (Physics.Raycast(ray, out hit)) { targetPos = hit.point; }
        else { targetPos = ray.origin + ray.direction * _currSkill.spawnDistance; }

        // calcs the direction from player to spawn position
        Vector3 direction = (targetPos - _inputSource.transform.position).normalized;

        // --> here
        _currSkill.prefab.GetComponent<BasePlayerSkill>().SpawnSkill();

        Vector3 spawnPos = _inputSource.transform.position + direction * _currSkill.spawnDistance;

        Instantiate(_currSkill.prefab, spawnPos, Quaternion.identity);

        GetRandomSkill();
    }

    // get a random index for the next skill
    private void GetRandomSkill() {
        if (_skillBlueprints.Length == 0) {
            return;
        }

        _currSkill = _skillBlueprints[Random.Range(0, _skillBlueprints.Length)];
    }
}
