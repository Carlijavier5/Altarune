using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerWheel : MonoBehaviour
{
    [SerializeField] private SummonController summonController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bigWheel;
    [SerializeField] private Transform smallWheel;
    [SerializeField] private Sprite defaultBg;
    [SerializeField] private Sprite selectBg;

    private int currentSelectionIndex = 0;

    private readonly Dictionary<SummonData, int> summonAngleDict = new();
    private readonly Dictionary<SummonData, int> summonIndexDict = new();

    private void Start() {
        summonController.OnSummonSelected += HandleSummonSelected;
        summonController.OnRosterSetup += HandleRosterSetup;
    }

    private void OnEnable() {
        if (summonController != null) {
            summonController.OnSummonSelected += HandleSummonSelected;
            summonController.OnRosterSetup += HandleRosterSetup;
        }
    }

    private void OnDisable() {
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
            if (summonAngleDict.ContainsKey(d)) {
                summonAngleDict.Remove(d);
                summonAngleDict.Add(d, 45 * (i - 1));
                continue;
            }

            summonAngleDict.Add(d, 45 * (i - 1));
            i++;
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

        i = 0;
        foreach (Transform t in bigWheel.transform) {
            if (!data[i]) {
                break;
            }
            t.GetChild(0).GetComponent<Image>().sprite = data[i].icon;
            i++;
        }
    }

    private Coroutine rotateWheelCoroutine;

    private void HandleSummonSelected(SummonType type, SummonData data) {
        bigWheel.GetChild(currentSelectionIndex).GetComponent<Image>().sprite = defaultBg;

        if (type == SummonType.None) {
            Hide();
            return;
        }

        summonIndexDict.TryGetValue(data, out currentSelectionIndex);

        bigWheel.GetChild(currentSelectionIndex).GetComponent<Image>().sprite = selectBg;

        if (summonAngleDict.TryGetValue(data, out int targetRotation)) {
            if (rotateWheelCoroutine != null) {
                StopCoroutine(rotateWheelCoroutine);
            }
            rotateWheelCoroutine = StartCoroutine(RotateWheelToAngle(targetRotation));
        }

        animator.SetBool("isSelect", true);
    }

    private IEnumerator RotateWheelToAngle(float targetAngle) {
        float duration = 1f;
        float elapsedTime = 0f;
        float startAngle = bigWheel.localEulerAngles.z;

        if (Mathf.Abs(targetAngle - startAngle) > 180f) {
            if (targetAngle > startAngle) {
                startAngle += 360f;
            }
            else {
                targetAngle += 360f;
            }
        }

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, elapsedTime / duration);
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

    void LateUpdate() {
        foreach (Transform child in bigWheel) {
            child.localEulerAngles = -bigWheel.localEulerAngles;
        }
    }

    private void Hide() {
        animator.SetBool("isSelect", false);
    }
}
