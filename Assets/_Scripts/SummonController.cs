using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonController : MonoBehaviour {

    [SerializeField] private Camera mainCamera;

    private enum SelectionType { None = 0, Battery, Tower }
    private SelectionType selectedType;

    [SerializeField] private TowerData[] towerBlueprints;
    [SerializeField] private BatteryData batteryData;

    public PlayerInput playerInput;

    private Vector3 prevMousePos;
    private IEnumerable<Battery> hintBatteries;
    private Battery hintBattery;
    private Tower hintTower;

    private int selectedSlot = 0;

    void Awake() {
        playerInput = new();
        playerInput.Actions.Enable();
        playerInput.Actions.Summon.performed += Summon_performed; ;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            SetSelectionType(selectedType == SelectionType.Battery ? SelectionType.None : SelectionType.Battery);
        } else if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SetSelectionType(selectedType == SelectionType.Tower? SelectionType.None : SelectionType.Tower);
        }

        if (selectedType != 0) {
            if (prevMousePos != Input.mousePosition) {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                IEnumerable<RaycastHit> hitInfo;
                if ((hitInfo = Physics.RaycastAll(ray).Where((info) => info.normal == Vector3.up)).Count() > 0) {
                    hintBatteries = hitInfo.Select((info) => info.collider.GetComponent<Battery>())
                                           .Where((item) => item != null);
                    switch (selectedType) {
                        case SelectionType.Battery:
                            if (hintBattery) {
                                hintBattery.transform.position = hitInfo.First().point;
                            } else {
                                hintBattery = Instantiate(batteryData.prefab);
                                hintBattery.HalfFade();
                            }
                            break;
                        case SelectionType.Tower:
                            if (hintTower) {
                                hintTower.transform.position = hitInfo.First().point;
                            } else {
                                hintTower = Instantiate(towerBlueprints[selectedSlot].prefab);
                                hintTower.HalfFade();
                            }
                            break;
                    }
                } else DestroyHints();
            }
            prevMousePos = Input.mousePosition;
        }
    }

    private void SetSelectionType(SelectionType selectionType) {
        DestroyHints();
        prevMousePos = Vector3.zero;
        selectedType = selectionType;
    }

    private void DestroyHints() {
        if (hintBattery) Destroy(hintBattery.gameObject);
        if (hintTower) Destroy(hintTower.gameObject);
    }

    private void Summon_performed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        if (context.performed && selectedType != 0) {
            foreach (Battery battery in hintBatteries) {

            }
            switch (selectedType) {
                case SelectionType.Battery:
                    break;
                case SelectionType.Tower:
                    break;
            }
        }
    }
}