using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SovereignStaticLaserMaster : SovereignPhaseMaster<StaticLaserProperties> {

    public event System.Action OnAttackEnd;

    [SerializeField] private SovereignStaticLaser[] laserArray;

    private readonly HashSet<SovereignStaticLaser> activeLasers = new();
    private readonly Dictionary<SovereignStaticLaser, int> laserIndexMap = new();

    private SwipeDirection prevDirection;
    private int waveCounter;

    protected override void Awake() {
        base.Awake();
        for (int i = 0; i < laserArray.Length; i++) {
            SovereignStaticLaser laser = laserArray[i];
            laser.OnLaserEnd += Laser_OnLaserEnd;
            laserIndexMap[laser] = i;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            EnterPhase(SovereignPhase.Macro1);
            DoAttack();
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            EnterPhase(SovereignPhase.Macro2);
            DoAttack();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            EnterPhase(SovereignPhase.Macro3);
            DoAttack();
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            EnterPhase(SovereignPhase.Macro4);
            DoAttack();
        }
    }

    public void DoAttack() {
        waveCounter = activeConfig.waveAmount;
        DoLaserWave();
    }

    private void DoLaserWave() {
        waveCounter--;
        prevDirection = prevDirection == SwipeDirection.LeftRight ? SwipeDirection.RightLeft
                                                                  : SwipeDirection.LeftRight;
        List<SovereignStaticLaser> availableLasers = laserArray.Except(activeLasers).ToList();

        List<SovereignStaticLaser> pickList = new();

        SovereignStaticLaser laserPick;
        int pickIndex, actualIndex, laserCount = activeConfig.laserAmount;
        while (availableLasers.Count > 0 && laserCount > 0) {
            pickIndex = Random.Range(0, availableLasers.Count);
            laserPick = availableLasers[pickIndex];
            availableLasers.RemoveAt(pickIndex);

            actualIndex = laserIndexMap[laserPick];

            bool isLeftEnd = actualIndex == 0,
                 isRightEnd = actualIndex == laserArray.Length - 1,
                 leftAdjacency = isLeftEnd || pickList.Contains(laserArray[actualIndex - 1]),
                 rightAdjacency = isRightEnd || pickList.Contains(laserArray[actualIndex + 1]);
            if (!(leftAdjacency && rightAdjacency)) {
                if (leftAdjacency || rightAdjacency) {
                    if (!isLeftEnd) availableLasers.Remove(laserArray[actualIndex - 1]);
                    if (!isRightEnd) availableLasers.Remove(laserArray[actualIndex + 1]);
                } pickList.Add(laserPick);
                laserCount--;
            }
        }
        pickList.Sort((x, y) => prevDirection == SwipeDirection.LeftRight ? laserIndexMap[x] - laserIndexMap[y]
                                                                          : laserIndexMap[y] - laserIndexMap[x]);
        StartCoroutine(ISpawnWave(pickList));
    }

    private IEnumerator ISpawnWave(List<SovereignStaticLaser> pickList) {
        SovereignStaticLaser laser;
        float warningTime, laserTime,
              timeStep = activeConfig.spawnTime / activeConfig.laserAmount;
        for (int i = 0; i < pickList.Count; i++) {
            warningTime = activeConfig.warningTime + timeStep * i;
            laserTime = activeConfig.laserTime - timeStep * i;
            laser = pickList[i];
            laser.Activate(warningTime, laserTime);
            activeLasers.Add(laser);
        }
        if (waveCounter > 0) {
            yield return new WaitForSeconds(activeConfig.waveWait);
            DoLaserWave();
        }
    }

    private void Laser_OnLaserEnd(SovereignStaticLaser laser) {
        activeLasers.Remove(laser);
        if (activeLasers.Count == 0 && waveCounter == 0) {
            OnAttackEnd?.Invoke();
            OnAttackEnd = null;
        }
    }
}