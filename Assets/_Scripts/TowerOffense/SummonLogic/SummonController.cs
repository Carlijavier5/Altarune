using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonController : MonoBehaviour {

    [SerializeField] private Camera mainCamera;
    [SerializeField] private PHSelector phSelector;

    public enum SelectionType { None = 0, Battery, Tower }
    private SelectionType selectedType;

    [SerializeField] private TowerData[] towerBlueprints;
    [SerializeField] private BatteryData batteryData;

    private List<Battery> summonedBatteries = new();

    public PlayerInput playerInput;

    private Vector3 prevMousePos;

    private IEnumerable<Battery> hintBatteries;
    private Vector3 lastHitPoint;

    private Battery hintBattery;
    private Summon hintTower;

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
            SetSelectionType(selectedType == SelectionType.Tower ? SelectionType.None : SelectionType.Tower, 0);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SetSelectionType(selectedType == SelectionType.Tower ? SelectionType.None : SelectionType.Tower, 1);
        }

        if (selectedType != 0) {
            if (prevMousePos != Input.mousePosition) {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                IEnumerable<RaycastHit> hitInfo;
                if ((hitInfo = Physics.RaycastAll(ray)).Count() > 0) {
                    hintBatteries = hitInfo.Select((info) => info.collider.GetComponent<Battery>())
                                           .Where((item) => item != null);
                    hitInfo = hitInfo.Where((info) => !info.collider.isTrigger);
                    if (hitInfo.Count() > 0) {
                        switch (selectedType) {
                            case SelectionType.Battery:
                                if (hintBattery) {
                                    hintBattery.transform.position = hitInfo.First().point;
                                } else {
                                    hintBattery = Instantiate(batteryData.prefab);
                                    hintBattery.transform.position = hitInfo.First().point;
                                    hintBattery.ToggleHologram(true);
                                } lastHitPoint = hitInfo.First().point;
                                break;
                            case SelectionType.Tower:
                                if (hintTower) {
                                    hintTower.transform.position = hitInfo.First().point;
                                } else {
                                    hintTower = Instantiate(towerBlueprints[selectedSlot].prefab);
                                    hintTower.transform.position = hitInfo.First().point;
                                    hintTower.ToggleHologram(true);
                                } lastHitPoint = hitInfo.First().point;
                                hintTower.ToggleHologramRed(hintBatteries.Count() <= 0);
                                break;
                        }
                    }
                } else DestroyHints();
            }
            prevMousePos = Input.mousePosition;
        }
    }

    private void SetSelectionType(SelectionType selectionType, int index = 0) {
        DestroyHints();
        prevMousePos = Vector3.zero;
        selectedType = selectionType;
        phSelector.SetSelectedImage(selectionType, index);
        selectedSlot = index;

        switch (selectedType) {
            case SelectionType.None:
            case SelectionType.Battery:
                foreach (Battery battery in summonedBatteries) {
                    battery.ToggleArea(false);
                }
                break;
            case SelectionType.Tower:
                foreach (Battery battery in summonedBatteries) {
                    battery.ToggleArea(true);
                }
                break;
        }
    }

    private void DestroyHints() {
        if (hintBattery) Destroy(hintBattery.gameObject);
        if (hintTower) Destroy(hintTower.gameObject);
    }

    private void Summon_performed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        if (context.performed && selectedType != 0) {
            switch (selectedType) {
                case SelectionType.Battery:
                    Battery battery = Instantiate(batteryData.prefab, lastHitPoint, Quaternion.identity);
                    summonedBatteries.Add(battery);
                    battery.DoSpawnAnim();
                    SetSelectionType(SelectionType.None);
                    break;
                case SelectionType.Tower:
                    if (hintBatteries.Count() > 0) {
                        Summon tower = Instantiate(towerBlueprints[selectedSlot].prefab, lastHitPoint, Quaternion.identity);
                        tower.DoSpawnAnim();
                        SetSelectionType(SelectionType.None);
                        tower.Init();
                    }
                    break;
            }
        }
    }
}