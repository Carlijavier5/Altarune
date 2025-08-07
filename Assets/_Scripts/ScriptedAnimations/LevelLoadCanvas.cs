using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelLoadCanvas : MonoBehaviour {
    [SerializeField] private CCondition condition;
    [SerializeField] private float initOffset = -10f;
    
    [SerializeField] private Transform level;
    private Vector3 levelInitPos;

    [SerializeField] private Transform divider;
    private Vector3 dividerInitPos;
    
    [SerializeField] private Transform floor;
    private Vector3 floorInitPos;

    [Header("Timing")] [SerializeField] private float initDelay = 1f;
    [SerializeField] private float animationSpeed = 0.5f;
    [SerializeField] private float staggerTime = 0.3f;
    [SerializeField] private float displayTime = 3f;
    
    // Start is called before the first frame update
    void Start() {
        levelInitPos = level.position;
        dividerInitPos = divider.position;
        floorInitPos = floor.position;

        level.DOMoveX(initOffset, 0f);
        divider.DOMoveX(initOffset, 0f);
        floor.DOMoveX(initOffset, 0f);

        GM.DialogueManager.OnDialogueEnd += DisplayAction;
    }

    private void DisplayAction() {
        if (condition.ConditionIsMet()) StartCoroutine(Display());
    }

    private IEnumerator Display() {
        yield return new WaitForSeconds(initDelay);
        level.DOMove(levelInitPos, animationSpeed).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(staggerTime);
        divider.DOMove(dividerInitPos, animationSpeed).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(staggerTime);
        floor.DOMove(floorInitPos, animationSpeed).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(displayTime);
        
        level.DOMoveX(initOffset, animationSpeed).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(staggerTime);
        divider.DOMoveX(initOffset, animationSpeed).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(staggerTime);
        floor.DOMoveX(initOffset, animationSpeed).SetEase(Ease.InCubic);
        yield return null;
    }
}
