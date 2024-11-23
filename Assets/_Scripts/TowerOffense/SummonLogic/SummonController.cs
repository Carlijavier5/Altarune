using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SummonType { None = 0, Battery, Tower }

public class SummonController : MonoBehaviour {

    public event System.Action<SummonData[]> OnRosterSetup;
    public event System.Action<SummonType, SummonData> OnSummonSelected;
    public event System.Action<Ray> OnRaycastUpdate;
    public event System.Action<SummonType> OnPointerConfirm;

    [SerializeField] private PlayerController inputSource;
    [SerializeField] private Entity summoner;
    [SerializeField] private ManaSource manaSource;

    [SerializeField] private PHSelector phSelector;

    [SerializeField] private TowerData[] towerBlueprints;
    [SerializeField] private BatteryData batteryData;

    public Entity Summoner => summoner;
    public ManaSource ManaSource => manaSource;

    private SummonType selectedType;
    private Vector2 prevMousePos;

    void Awake() {
        inputSource.OnPlayerInit += InputSource_OnPlayerInit;
    }

    private void InputSource_OnPlayerInit() {
        inputSource.OnSummonPerformed += InputSource_OnPointerConfirm;
        inputSource.OnSummonSelect += InputSource_OnSummonSelect;
        OnRosterSetup?.Invoke(new SummonData[] { batteryData }
                             .Concat(towerBlueprints).ToArray());
    }

    void Update() {
        if (selectedType != 0) {
            if (prevMousePos != inputSource.CursorPosition) RaycastUpdate();
            prevMousePos = inputSource.CursorPosition;
        }
    }

    private void InputSource_OnSummonSelect(SummonType selectionType, int slotNum) {
        switch (selectionType) {
            case SummonType.None:
                SetSelectionType(SummonType.None);
                break;
            case SummonType.Battery:
                SetSelectionType(selectedType == SummonType.Battery ? SummonType.None : SummonType.Battery);
                break;
            case SummonType.Tower:
                SetSelectionType(selectedType == SummonType.Tower ? SummonType.None : SummonType.Tower, slotNum - 1);
                break; 
        }
    }

    public void RaycastUpdate() {
        Ray cursorRay = inputSource.OutputCamera.ScreenPointToRay(inputSource.CursorPosition);
        OnRaycastUpdate?.Invoke(cursorRay);
    }

    private void InputSource_OnPointerConfirm() => OnPointerConfirm?.Invoke(selectedType);

    private void SetSelectionType(SummonType selectionType, int index = 0) {
        StartCoroutine(DelayCast());
        selectedType = selectionType;

        SummonData summonData = selectedType == SummonType.Battery ? batteryData
                              : selectedType == SummonType.Tower ? towerBlueprints[index]
                              : null;
        OnSummonSelected?.Invoke(selectedType, summonData);
        phSelector.SetSelectedImage(selectionType, index);
    }

    private IEnumerator DelayCast() {
        yield return new WaitForEndOfFrame();
        RaycastUpdate();
    }

    public void ClearSelection() => SetSelectionType(SummonType.None);
}