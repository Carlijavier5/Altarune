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

    private bool skillHeld = false;

    void Awake() {
        _inputSource = GetComponentInParent<PlayerController>();
        _inputSource.OnPlayerInit += InputSource_OnPlayerInit;
    }

    void Start() { GetRandomSkill(); }
    void Update() { if (skillHeld) DrawSkillLine(); }

    private void InputSource_OnPlayerInit() {
        _inputSource.OnSkillStarted += SkillStarted;
        _inputSource.OnSkillCast += CastSkill;
    }

    /// <summary>
    /// before you cast, apply the vision line to see the direction of the skill
    /// </summary>
    private void SkillStarted() { skillHeld = true; }

    private void DrawSkillLine() {
        Debug.Log("line");
    }

    private void CastSkill() {
        skillHeld = false;
        // code here for declining the skill use if you don't have enough energ

        // summon the prefab in direction of the mouse
        LayerMask layerMask = LayerMask.GetMask("SkillRaycasts");

        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        Vector3 og_targetPos;
        // check if the skill collides with something else (idk if we want to keep this)
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) { og_targetPos = hit.point; }
        else {
            //og_targetPos = ray.origin + ray.direction * _currSkill.spawnDistance; 

            // If there's no hit, calculate intersection with an isometric plane at targetHeight
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
            float distance;
            if (groundPlane.Raycast(ray, out distance)) {
                og_targetPos = ray.GetPoint(distance);
            }
            else {
                // Fallback position if no plane intersection
                og_targetPos = ray.origin + ray.direction * _currSkill.spawnDistance;
            }
        }

        // calcs the direction from player to spawn position
        Vector3 direction = (og_targetPos - _inputSource.transform.position).normalized;

        Vector3 constained_targetPos = _inputSource.transform.position + direction * _currSkill.spawnDistance;

        // spawn skill
        _currSkill.prefab.GetComponent<BasePlayerSkill>().SpawnSkill(_currSkill, this.gameObject.transform.position, constained_targetPos, null, null);

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
