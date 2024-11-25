using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerWheel : MonoBehaviour
{
    [SerializeField] private SummonController summonController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bigWheel;

    private Dictionary<SummonData, int> summonAngleDict = new();

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

        summonAngleDict = new Dictionary<SummonData, int>();

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

        int j = 0;
        foreach (Transform t in bigWheel.transform) {
            if (!data[j]) {
                break;
            }
            t.GetChild(0).GetComponent<Image>().sprite = data[j].icon;
            j++;
        }
    }

    private void HandleSummonSelected(SummonType type, SummonData data) {
        if (type == SummonType.None) {
            Hide();
            return;
        }

        if (summonAngleDict.TryGetValue(data, out int rot)) {
            bigWheel.eulerAngles = new Vector3(bigWheel.rotation.x, bigWheel.rotation.y, rot);
        }

        animator.SetBool("isSelect", true);
    }

    private void Hide() {
        animator.SetBool("isSelect", false);
    }
}
