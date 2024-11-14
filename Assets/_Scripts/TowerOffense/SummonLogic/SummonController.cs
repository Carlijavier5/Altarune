using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SummonType { None = 0, Battery, Tower }

public class SummonController : MonoBehaviour {

    public event System.Action<TowerData> OnTowerSelected;
    public event System.Action<BatteryData> OnBatterySelected;

    [SerializeField] private PlayerController inputSource;
    [SerializeField] private Entity summoner;
    [SerializeField] private ManaSource manaSource;

    [SerializeField] private PHSelector phSelector;

    [SerializeField] private TowerData[] towerBlueprints;
    [SerializeField] private BatteryData batteryData;

    // is this supposed to be here
    // reply: not really lol, but it's 100% ok, we'll take it :D
    [SerializeField] private Material manaConnectionMaterial;
    [SerializeField] private float batteryCost, towerCost;
    [SerializeField] private float overlapRadius;

    private readonly HashSet<ArtificialBattery> summonedBatteries = new();

    private SummonType selectedType;
    private Vector2 prevMousePos;

    private class ClosestBatteryCache {
        public IBattery battery;
        public float distance;
    } private ClosestBatteryCache closestBatteryCache;
    private Vector3 lastHitPoint;

    private SummonHologram summonHint;

    private HashSet<GameObject> overlapSummons;
    private bool IsPlacementInvalid => (selectedType == SummonType.Tower && closestBatteryCache == null)
                                    || (overlapSummons == null || overlapSummons.Count > 0);

    private int selectedSlot = 0;

    void Awake() {
        inputSource.OnPlayerInit += InputSource_OnPlayerInit;
    }

    private void InputSource_OnPlayerInit() {
        inputSource.OnSummonPerformed += InputSource_OnSummonPerformed;
        inputSource.OnSummonSelect += InputSource_OnSummonSelect;
        manaSource.OnManaCollapse += Player_OnManaCollapse;
    }

    private void Player_OnManaCollapse() {
        foreach (ArtificialBattery battery in summonedBatteries) {
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
        if (Physics.Raycast(ray, out RaycastHit groundHit, Mathf.Infinity, LayerUtils.GroundLayerMask)
            && Mathf.Max(groundHit.normal.x, groundHit.normal.y, groundHit.normal.z) == groundHit.normal.y) {

            switch (selectedType) {
                case SummonType.Battery:
                    UpdateHint(batteryData.prefabHologram, groundHit.point);
                    break;
                case SummonType.Tower:
                    if ((objectsHit = Physics.RaycastAll(ray, Mathf.Infinity, LayerUtils.SummonHintLayerMask)).Count() > 0) {
                        foreach (RaycastHit hitInfo in objectsHit) {
                            if (hitInfo.collider.TryGetComponent(out BatteryArea batteryArea) && batteryArea.IsActive) {
                                IBattery battery = batteryArea.Battery;
                                float distance = Vector3.Distance(groundHit.point, battery.Position);
                                if (closestBatteryCache == null || distance > closestBatteryCache.distance) {
                                    closestBatteryCache = new() { battery = battery, distance = distance };
                                }
                            }
                        }
                    } else closestBatteryCache = null;

                    UpdateHint(towerBlueprints[selectedSlot].prefabHologram, groundHit.point);
                    break;
            }

            if (selectedType != 0 && summonHint) {
                Collider[] colliders = Physics.OverlapSphere(summonHint.transform.position, overlapRadius,
                                                             LayerUtils.SummonHintLayerMask);
                overlapSummons = new();
                foreach (Collider collider in colliders) {
                    if (collider.TryGetComponent(out Summon summon)
                        && Vector3.Distance(summon.transform.position, groundHit.point) < overlapRadius) {
                        overlapSummons.Add(summon.gameObject);
                    }
                }

                summonHint.ToggleHologramRed(IsPlacementInvalid);
            }
        } else DestroyHint();
    }

    private void UpdateHint(SummonHologram summonPrefab, Vector3 groundPoint) {
        if (summonHint) {
            summonHint.transform.position = groundPoint;
        } else {
            summonHint = Instantiate(summonPrefab);
            summonHint.transform.position = groundPoint;
        } lastHitPoint = groundPoint;
    }

    private void SetSelectionType(SummonType selectionType, int index = 0) {
        DestroyHint();
        StartCoroutine(DelayCast());

        selectedType = selectionType;
        phSelector.SetSelectedImage(selectionType, index);
        selectedSlot = index;

        switch (selectedType) {
            case SummonType.None:
            case SummonType.Battery:
                OnBatterySelected?.Invoke(batteryData);
                foreach (ArtificialBattery battery in summonedBatteries) {
                    battery.ToggleArea(false);
                } break;
            case SummonType.Tower:
                OnTowerSelected?.Invoke(towerBlueprints[index]);
                foreach (ArtificialBattery battery in summonedBatteries) {
                    battery.ToggleArea(true);
                } break;
        }
    }

    private IEnumerator DelayCast() {
        yield return new WaitForEndOfFrame();
        RaycastSummon();
    }

    private void DestroyHint() {
        if (summonHint) Destroy(summonHint.gameObject);
        overlapSummons = new();
    }

    private void InputSource_OnSummonPerformed() {
        if (selectedType != 0 && summonHint != null
            && !IsPlacementInvalid) {
            switch (selectedType) {
                case SummonType.Battery:
                    ArtificialBattery battery = Instantiate(batteryData.prefabSummon, lastHitPoint, Quaternion.identity);

                    summonedBatteries.Add(battery);
                    battery.DoSpawnAnim();

                    battery.Init(summoner, manaSource);
                    manaSource.Drain(batteryCost);

                    SetSelectionType(SummonType.None);
                    break;
                case SummonType.Tower:
                    if (closestBatteryCache != null) {
                        Summon tower = Instantiate(towerBlueprints[selectedSlot].prefabSummon, lastHitPoint, Quaternion.identity);

                        IBattery targetBattery = closestBatteryCache.battery;
                        ManaSource batterySource = targetBattery.ManaSource;

                        targetBattery.LinkTower(tower);
                        tower.DoSpawnAnim();

                        tower.Init(summoner, batterySource);
                        batterySource.Drain(towerCost);

                        SetSelectionType(SummonType.None);
                        CreateManaConnection(tower.transform, targetBattery.MonoScript.transform);
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

    private void CreateManaConnection(Transform batteryTransform, Transform towerTransform) {
        GameObject lineObject = new GameObject("ManaConnection");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;

        Vector3 batteryConnectionPoint = batteryTransform.position;
        batteryConnectionPoint.y += 0.5f;
        Vector3 towerConnectionPoint = towerTransform.position;
        towerConnectionPoint.y += 0.5f;

        lineRenderer.SetPosition(0, batteryConnectionPoint);
        lineRenderer.SetPosition(1, towerConnectionPoint);

        lineRenderer.startWidth = 0.75f;
        lineRenderer.endWidth = 0.75f;
        lineRenderer.material = this.manaConnectionMaterial;
    }
}