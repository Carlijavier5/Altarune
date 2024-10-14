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
        playerInput.Actions.Summon.performed += Summon_Performed; ;
    }

    void Update() {
        if (selectedType != 0) {
            if (prevMousePos != Input.mousePosition) {
                RaycastSummon();
            }
            prevMousePos = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            SetSelectionType(selectedType == SelectionType.Battery ? SelectionType.None : SelectionType.Battery);
        } else if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SetSelectionType(selectedType == SelectionType.Tower ? SelectionType.None : SelectionType.Tower, 0);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SetSelectionType(selectedType == SelectionType.Tower ? SelectionType.None : SelectionType.Tower, 1);
        }
    }

    private void RaycastSummon() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        IEnumerable<RaycastHit> objectsHit;

        if (Physics.Raycast(ray, out RaycastHit groundHit, Mathf.Infinity, 1 << 7)
            && Mathf.Max(groundHit.normal.x, groundHit.normal.y, groundHit.normal.z) == groundHit.normal.y) {

            switch (selectedType) {
                case SelectionType.Battery:
                    if (hintBattery) {
                        hintBattery.transform.position = groundHit.point;
                    } else {
                        hintBattery = Instantiate(batteryData.prefab);
                        hintBattery.transform.position = groundHit.point;
                        hintBattery.ToggleHologram(true);
                    } lastHitPoint = groundHit.point;
                    break;
                case SelectionType.Tower:
                    if ((objectsHit = Physics.RaycastAll(ray, Mathf.Infinity, 1 << 8)).Count() > 0) {
                        hintBatteries = objectsHit.Select(info => info.collider.GetComponent<Battery>());
                    } else hintBatteries = null;

                    if (hintTower) {
                        hintTower.transform.position = groundHit.point;
                    } else {
                        hintTower = Instantiate(towerBlueprints[selectedSlot].prefab);
                        hintTower.transform.position = groundHit.point;
                        hintTower.ToggleHologram(true);
                    } lastHitPoint = groundHit.point;
                    hintTower.ToggleHologramRed(hintBatteries == null);
                    break;
            }
        } else DestroyHints();
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
                } break;
            case SelectionType.Tower:
                foreach (Battery battery in summonedBatteries) {
                    battery.ToggleArea(true);
                } break;
        }
    }

    private void DestroyHints() {
        if (hintBattery) Destroy(hintBattery.gameObject);
        if (hintTower) Destroy(hintTower.gameObject);
    }

    private void Summon_Performed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        if (context.performed && selectedType != 0) {
            switch (selectedType) {
                case SelectionType.Battery:
                    Battery battery = Instantiate(batteryData.prefab, lastHitPoint, Quaternion.identity);
                    summonedBatteries.Add(battery);
                    battery.DoSpawnAnim();
                    SetSelectionType(SelectionType.None);
                    break;
                case SelectionType.Tower:
                    if (hintBatteries != null) {
                        Summon tower = Instantiate(towerBlueprints[selectedSlot].prefab, lastHitPoint, Quaternion.identity);
                        tower.DoSpawnAnim();
                        SetSelectionType(SelectionType.None);
                        tower.Init();
                    } break;
            }
        }
    }
}