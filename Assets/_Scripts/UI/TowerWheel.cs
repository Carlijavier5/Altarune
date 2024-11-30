using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerWheel : MonoBehaviour
{
    [SerializeField] private SummonController summonController;
    [SerializeField] private TowerWheelSlot[] towerSlots;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bigWheel, smallWheel;
    [SerializeField] private float rotateDuration;

    private int currentSelectionIndex = 0;

    private readonly Dictionary<SummonData, int> summonAngleDict = new();
    private readonly Dictionary<SummonData, int> summonIndexDict = new();

    void OnEnable() {
        if (summonController != null) {
            summonController.OnSummonSelected += HandleSummonSelected;
            summonController.OnRosterSetup += HandleRosterSetup;
        }
    }

    void OnDisable() {
        if (summonController != null) {
            summonController.OnSummonSelected -= HandleSummonSelected;
            summonController.OnRosterSetup -= HandleRosterSetup;
        }
    }

    private void HandleRosterSetup(SummonData[] data) {
        if (data.Length == 0) {
            return;
        }

        summonAngleDict.Clear();

        int i = 0;
        foreach (SummonData d in data) {
            if (i < 8) {
                towerSlots[i].Init(d);
                towerSlots[i].Toggle(true);
            }

            if (summonAngleDict.ContainsKey(d)) {
                summonAngleDict.Remove(d);
                summonAngleDict.Add(d, 45 * (i - 1));
                continue;
            }

            summonAngleDict.Add(d, 45 * (i - 1));
            i++;
        }

        for (int j = i; j < 8; j++) {
            towerSlots[i].Toggle(false);
        }

        summonIndexDict.Clear();

        i = 0;
        foreach (SummonData d in data) {
            if (summonIndexDict.ContainsKey(d)) {
                summonIndexDict.Remove(d);
                summonIndexDict.Add(d, i);
                continue;
            }

            summonIndexDict.Add(d, i);
            i++;
        }
    }

    private Coroutine rotateWheelCoroutine;

    private void HandleSummonSelected(SummonType type, SummonData data) {
        if (IsWithinSlots(currentSelectionIndex)) {
            towerSlots[currentSelectionIndex].ToggleSelection(false);
        }

        if (type == SummonType.None) {
            Hide();
            return;
        }

        bool hasSelection = summonIndexDict.TryGetValue(data, out currentSelectionIndex)
                            && IsWithinSlots(currentSelectionIndex);
        if (hasSelection) {
            towerSlots[currentSelectionIndex].ToggleSelection(true);

            if (summonAngleDict.TryGetValue(data, out int targetRotation)) {
                if (rotateWheelCoroutine != null) {
                    StopCoroutine(rotateWheelCoroutine);
                }
                rotateWheelCoroutine = StartCoroutine(RotateWheelToAngle(targetRotation));
            }
            animator.SetBool("isSelect", true);
        } else {
            Hide();
        }
    }

    private bool IsWithinSlots(int index) {
        return currentSelectionIndex < towerSlots.Length
               && currentSelectionIndex >= 0;
    }

    private IEnumerator RotateWheelToAngle(float targetAngle) {
        float elapsedTime = 0;
        float startAngle = bigWheel.localEulerAngles.z;

        if (Mathf.Abs(targetAngle - startAngle) > 180f) {
            if (targetAngle > startAngle) {
                startAngle += 360f;
            }
            else {
                targetAngle += 360f;
            }
        }

        while (elapsedTime < rotateDuration) {
            elapsedTime += Time.deltaTime;
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, elapsedTime / rotateDuration);
            bigWheel.localEulerAngles = new Vector3(bigWheel.localEulerAngles.x, bigWheel.localEulerAngles.y, currentAngle);
            smallWheel.localEulerAngles = new Vector3(smallWheel.localEulerAngles.x, smallWheel.localEulerAngles.y, -currentAngle);
            foreach (Transform child in bigWheel) {
                child.localEulerAngles = -bigWheel.localEulerAngles;
            }
            yield return null;
        }

        bigWheel.localEulerAngles = new Vector3(bigWheel.localEulerAngles.x, bigWheel.localEulerAngles.y, targetAngle);
        smallWheel.localEulerAngles = new Vector3(smallWheel.localEulerAngles.x, smallWheel.localEulerAngles.y, -targetAngle);
        foreach (Transform child in bigWheel) {
            child.localEulerAngles = -bigWheel.localEulerAngles;
        }
        rotateWheelCoroutine = null;
    }

    private void Hide() {
        animator.SetBool("isSelect", false);
    }
}