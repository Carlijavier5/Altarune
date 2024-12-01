using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonHandler : MonoBehaviour {

    public event System.Action<SummonData> OnSummonSuccess;

    [SerializeField] private SummonController inputSource;
    [SerializeField] private SummonData[] summonBlueprints;

    [SerializeField] private float summonCD, overlapRadius;

    private ManaSource ManaSource => inputSource.ManaSource;

    private readonly Dictionary<SummonData, SummonHologram> hologramMap = new();

    private readonly HashSet<ArtificialBattery> summonedBatteries = new();

    private class ClosestBatteryCache {
        public IBattery battery;
        public float distance;
    }
    private ClosestBatteryCache closestBatteryCache;
    private Vector3 lastHitPoint;

    private SummonData selectedData;
    private SummonHologram hologramHint;

    private HashSet<GameObject> overlapSummons;
    private bool IsPlacementInvalid => (selectedData is TowerData && closestBatteryCache == null)
                                    || (overlapSummons == null || overlapSummons.Count > 0);

    private float cdTime;

    void Awake() {
        inputSource.OnSummonSelected += SummonController_OnSummonSelected;
        inputSource.OnRaycastUpdate += InputSource_OnRaycastUpdate;
        inputSource.OnPointerConfirm += InputSource_OnPointerConfirm;
        ManaSource.OnManaCollapse += ManaSource_OnManaCollapse;

        /// Pre-warm summon holograms;
        foreach (SummonData summonData in summonBlueprints) {
            SummonHologram summonHologram = Instantiate(summonData.prefabHologram);
            hologramMap[summonData] = summonHologram;
            summonHologram.gameObject.SetActive(false);
        }
    }

    private void SummonController_OnSummonSelected(SummonType summonType, SummonData summonData) {
        selectedData = summonData;
        ClearHint();

        switch (summonType) {
            case SummonType.None:
            case SummonType.Battery:
                foreach (ArtificialBattery battery in summonedBatteries) {
                    battery.ToggleArea(false);
                } break;
            case SummonType.Tower:
                foreach (ArtificialBattery battery in summonedBatteries) {
                    battery.ToggleArea(true);
                } break;
        }
    }

    private void InputSource_OnRaycastUpdate(Ray cursorRay) {
        if (Time.time > cdTime && selectedData != null
            && Physics.Raycast(cursorRay, out RaycastHit groundHit, LayerUtils.MAX_RCD, LayerUtils.GroundLayerMask)
            && Mathf.Max(groundHit.normal.x, groundHit.normal.y, groundHit.normal.z) == groundHit.normal.y) {

            if (selectedData is TowerData) {
                IEnumerable<RaycastHit> objectsHit;
                if ((objectsHit = Physics.RaycastAll(cursorRay, LayerUtils.MAX_RCD, LayerUtils.SummonHintLayerMask)).Count() > 0) {
                    foreach (RaycastHit hitInfo in objectsHit) {
                        if (hitInfo.collider.TryGetComponent(out BatteryArea batteryArea) && batteryArea.IsActive) {
                            IBattery battery = batteryArea.Battery;
                            float distance = Vector3.Distance(groundHit.point, battery.Position);
                            if (closestBatteryCache == null || distance < closestBatteryCache.distance) {
                                closestBatteryCache = new() { battery = battery, distance = distance };
                            }
                        }
                    }
                } else closestBatteryCache = null;
            }
            
            UpdateHint(groundHit.point);

            if (hologramHint) {
                Collider[] colliders = Physics.OverlapSphere(hologramHint.transform.position, overlapRadius,
                                                             LayerUtils.SummonHintLayerMask);
                overlapSummons = new();
                foreach (Collider collider in colliders) {
                    if (collider.TryGetComponent(out Summon summon)
                        && Vector3.Distance(summon.transform.position, groundHit.point) < overlapRadius) {
                        overlapSummons.Add(summon.gameObject);
                    }
                }

                hologramHint.ToggleHologramRed(IsPlacementInvalid);
            }
        } else ClearHint();
    }

    private void InputSource_OnPointerConfirm(SummonType selectedType) {
        if (hologramHint != null
                && !IsPlacementInvalid) {
            cdTime = Time.time + summonCD;
            switch (selectedType) {
                case SummonType.Battery:
                    BatteryData batteryData = selectedData as BatteryData;
                    ArtificialBattery battery = Instantiate(batteryData.prefabSummon, lastHitPoint, Quaternion.identity);

                    summonedBatteries.Add(battery);
                    battery.DoSpawn();

                    battery.Init(inputSource.Summoner, ManaSource);
                    ManaSource.Drain(batteryData.summonCost);

                    inputSource.ClearSelection();
                    OnSummonSuccess?.Invoke(batteryData);
                    break;
                case SummonType.Tower:
                    if (closestBatteryCache != null
                        && closestBatteryCache.battery != null
                            && closestBatteryCache.battery.IsActive) {
                        TowerData towerData = selectedData as TowerData;
                        Summon tower = Instantiate(towerData.prefabSummon, lastHitPoint, Quaternion.identity);

                        IBattery targetBattery = closestBatteryCache.battery;
                        ManaSource batterySource = targetBattery.ManaSource;

                        targetBattery.LinkTower(tower);
                        tower.DoSpawn();

                        tower.Init(inputSource.Summoner, batterySource);
                        batterySource.Drain(towerData.summonCost);

                        inputSource.ClearSelection();
                        OnSummonSuccess?.Invoke(towerData);
                    } break;
            }
        }
    }

    private void ManaSource_OnManaCollapse() {
        foreach (ArtificialBattery battery in summonedBatteries) {
            battery.Collapse();
        } summonedBatteries.Clear();
    }

    private void UpdateHint(Vector3 groundPoint) {
        if (selectedData != null) {
            if (!hologramHint) {
                hologramHint = hologramMap[selectedData];
                hologramHint.gameObject.SetActive(true);
            }
            hologramHint.transform.position = groundPoint;
            lastHitPoint = groundPoint;
        }
    }

    private void ClearHint() {
        closestBatteryCache = null;
        if (hologramHint) {
            hologramHint.gameObject.SetActive(false);
            hologramHint = null;
        }
    }
}