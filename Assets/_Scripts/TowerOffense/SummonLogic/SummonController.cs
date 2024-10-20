using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SummonType { None = 0, Battery, Tower }

public class SummonController : MonoBehaviour {

    [SerializeField] private PlayerController inputSource;
    [SerializeField] private Player player;
    [SerializeField] private PHSelector phSelector;

    private SummonType selectedType;

    [SerializeField] private TowerData[] towerBlueprints;
    [SerializeField] private BatteryData batteryData;

    [SerializeField] private float batteryCost, towerCost;

    private readonly HashSet<Battery> summonedBatteries = new();

    private Vector2 prevMousePos;

    private IEnumerable<Battery> hintBatteries;
    private Vector3 lastHitPoint;

    private Battery hintBattery;
    private Summon hintTower;

    private int selectedSlot = 0;

    void Awake() {
        inputSource.OnPlayerInit += InputSource_OnPlayerInit;
    }

    private void InputSource_OnPlayerInit() {
        inputSource.OnSummonPerformed += InputSource_OnSummonPerformed;
        inputSource.OnSummonSelect += InputSource_OnSummonSelect;
        player.OnManaCollapse += Player_OnManaCollapse;
    }

    private void Player_OnManaCollapse() {
        foreach (Battery battery in summonedBatteries) {
            battery.Collapse();
        } summonedBatteries.Clear();
    }

    void Update() {
        if (selectedType != 0) {
            if (prevMousePos != inputSource.CursorPosition) RaycastSummon();
            prevMousePos = inputSource.CursorPosition;
        }
    }

    private void RaycastSummon() {
        Ray ray = inputSource.OutputCamera.ScreenPointToRay(inputSource.CursorPosition);

        IEnumerable<RaycastHit> objectsHit;
        if (Physics.Raycast(ray, out RaycastHit groundHit, Mathf.Infinity, 1 << 4)
            && Mathf.Max(groundHit.normal.x, groundHit.normal.y, groundHit.normal.z) == groundHit.normal.y) {

            switch (selectedType) {
                case SummonType.Battery:
                    if (hintBattery) {
                        hintBattery.transform.position = groundHit.point;
                    } else {
                        hintBattery = Instantiate(batteryData.prefab);
                        hintBattery.transform.position = groundHit.point;
                        hintBattery.ToggleHologram(true);
                    } lastHitPoint = groundHit.point;
                    break;
                case SummonType.Tower:
                    if ((objectsHit = Physics.RaycastAll(ray, Mathf.Infinity, 1 << 6)).Count() > 0) {
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

    private void SetSelectionType(SummonType selectionType, int index = 0) {
        DestroyHints();
        StartCoroutine(DelayCast());

        selectedType = selectionType;
        phSelector.SetSelectedImage(selectionType, index);
        selectedSlot = index;

        switch (selectedType) {
            case SummonType.None:
            case SummonType.Battery:
                foreach (Battery battery in summonedBatteries) {
                    battery.ToggleArea(false);
                } break;
            case SummonType.Tower:
                foreach (Battery battery in summonedBatteries) {
                    battery.ToggleArea(true);
                } break;
        }
    }

    private IEnumerator DelayCast() {
        yield return new WaitForEndOfFrame();
        RaycastSummon();
    }

    private void DestroyHints() {
        if (hintBattery) Destroy(hintBattery.gameObject);
        if (hintTower) Destroy(hintTower.gameObject);
    }

    private void InputSource_OnSummonPerformed() {
        if (selectedType != 0) {
            switch (selectedType) {
                case SummonType.Battery:
                    if (hintBattery == null) return;
                    Battery battery = Instantiate(batteryData.prefab, lastHitPoint, Quaternion.identity);
                    summonedBatteries.Add(battery);
                    player.ManaSource -= batteryCost;
                    battery.DoSpawnAnim();
                    SetSelectionType(SummonType.None);
                    break;
                case SummonType.Tower:
                    if (hintTower == null) return;
                    if (hintBatteries != null) {
                        Summon tower = Instantiate(towerBlueprints[selectedSlot].prefab, lastHitPoint, Quaternion.identity);
                        hintBatteries.First().LinkTower(tower);
                        player.ManaSource -= towerCost;
                        tower.DoSpawnAnim();
                        SetSelectionType(SummonType.None);
                        tower.Init(player);
                    } break;
            }
        }
    }

    private void InputSource_OnSummonSelect(SummonType selectionType, int slotNum) {
        switch (selectionType) {
            case SummonType.None:
                break;
            case SummonType.Battery:
                SetSelectionType(selectedType == SummonType.Battery ? SummonType.None : SummonType.Battery);
                break;
            case SummonType.Tower:
                SetSelectionType(selectedType == SummonType.Tower ? SummonType.None : SummonType.Tower, slotNum - 1);
                break; 
        }
    }
}