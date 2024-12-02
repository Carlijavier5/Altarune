using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GolemSavage {

    private enum SavageAttack { Spin, Slam, MeteorHurl }

    private abstract class PhaseState : State<Savage_Input> {

        public abstract SavagePhase Phase { get; }

        public override void Enter(Savage_Input _) { }

        public override void Update(Savage_Input _) { }

        public override void Exit(Savage_Input _) { }

        public abstract void PickAttack(Savage_Input _);
    }
}

public partial class GolemSavage {

    private class PhaseState_Phase1 : PhaseState {

        public override SavagePhase Phase => SavagePhase.Phase1;

        private readonly SavageAttack[] attackPool
            = new[] { SavageAttack.Spin, SavageAttack.Slam, SavageAttack.MeteorHurl };
        private List<SavageAttack> attackList = new();

        public override void Enter(Savage_Input input) {
            base.Enter(input);
            attackList = new(attackPool);
        }

        public override void PickAttack(Savage_Input input) {
            if (attackList.Count == 0) attackList = new(attackPool);
            int attackIndex = Random.Range(0, attackList.Count);
            SavageAttack attackType = attackList[attackIndex];
            switch (attackType) {
                case SavageAttack.Spin:
                    input.microMachine.SetState(new State_Spin());
                    attackList.Remove(SavageAttack.Spin);
                    break;
                case SavageAttack.Slam:
                    input.microMachine.SetState(new State_Slam(Phase));
                    attackList.Remove(SavageAttack.Slam);
                    break;
                case SavageAttack.MeteorHurl:
                    input.microMachine.SetState(new State_MeteorHurl(Phase));
                    attackList.Remove(SavageAttack.MeteorHurl);
                    break;
            }
        }
    }
}

public partial class GolemSavage {

    [Header("Tornado Siftlings")]
    [SerializeField] private GolemSiftling[] earthTornadoSiftlings;

    private class PhaseState_Phase2 : PhaseState {

        public override SavagePhase Phase => SavagePhase.Phase2;

        public override void Enter(Savage_Input input) {
            GolemSavage gs = input.savage;
            foreach (GolemSiftling siftling in gs.earthTornadoSiftlings) {
                Vector3 spawnPos = SpatialUtils.RandomPointInXZRing(input.savage.transform.position,
                                                              input.savage.siftlingSpawnRadius, true);
                siftling.TryTeleport(spawnPos);
            }
        }

        public override void PickAttack(Savage_Input _) { }
    }
}

public partial class GolemSavage {

    [Header("Final Phase Siftlings")]
    [SerializeField] private GolemSiftling swiftlingPrefab;
    [SerializeField] private Transform farawaySpawnPoint;
    [SerializeField] private int maxSiftlings;
    [SerializeField] private float siftlingSpawnInterval;
    [SerializeField] private Vector2 siftlingSpawnRadius;

    private class PhaseState_Phase3 : PhaseState {

        public override SavagePhase Phase => SavagePhase.Phase3;

        private readonly SavageAttack[] attackPool
            = new[] { SavageAttack.Spin, SavageAttack.Slam, SavageAttack.MeteorHurl };
        private List<SavageAttack> attackList = new();

        private readonly HashSet<GolemSiftling> spawnedSiftlings = new();
        private float timer;

        public override void Update(Savage_Input input) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                timer = input.savage.siftlingSpawnInterval;
                TrySpawnSiftling(input);
            }
        }

        public override void Exit(Savage_Input _) {
            foreach (GolemSiftling spawnedSiftling in spawnedSiftlings) {
                spawnedSiftling.OnPerish -= SpawnedSiftling_OnPerish;
                spawnedSiftling.Perish();
            }
        }

        public override void PickAttack(Savage_Input input) {
            if (attackList.Count == 0) attackList = new(attackPool);
            int attackIndex = Random.Range(0, attackList.Count);
            SavageAttack attackType = attackList[attackIndex];
            switch (attackType) {
                case SavageAttack.Spin:
                    input.microMachine.SetState(new State_Spin());
                    attackList.Remove(SavageAttack.Spin);
                    break;
                case SavageAttack.Slam:
                    input.microMachine.SetState(new State_Slam(Phase));
                    attackList.Remove(SavageAttack.Slam);
                    break;
                case SavageAttack.MeteorHurl:
                    input.microMachine.SetState(new State_MeteorHurl(Phase));
                    attackList.Remove(SavageAttack.MeteorHurl);
                    break;
            }
        }

        private void TrySpawnSiftling(Savage_Input input) {
            if (spawnedSiftlings.Count < input.savage.maxSiftlings) {
                Transform t = input.savage.farawaySpawnPoint;
                GolemSiftling spawnedSiftling = Instantiate(input.savage.swiftlingPrefab,
                                                            t.position, t.rotation);
                spawnedSiftling.OnPerish += SpawnedSiftling_OnPerish;
                spawnedSiftlings.Add(spawnedSiftling);
                input.savage.StartCoroutine(IDelaySiftlingTeleport(input, spawnedSiftling));
            }
        }

        private IEnumerator IDelaySiftlingTeleport(Savage_Input input, GolemSiftling siftling) {
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(1);
            Vector3 spawnPos = SpatialUtils.RandomPointInXZRing(input.savage.transform.position,
                                                    input.savage.siftlingSpawnRadius, true);
            siftling.TryTeleport(spawnPos);
        }

        private void SpawnedSiftling_OnPerish(BaseObject baseObject) {
            if (baseObject is GolemSiftling) {
                GolemSiftling spawnedSiftling = baseObject as GolemSiftling;
                spawnedSiftlings.Remove(spawnedSiftling);
            }
            baseObject.OnPerish -= SpawnedSiftling_OnPerish;
        }
    }
}

[System.Serializable]
public class SavagePhaseConfiguration {
    public SavagePhase savagePhase;
    [Header("General")]
    public int health;
    public Vector2 attackTimeRange;
    public float roamSpeed;
    [Header("Spin")]
    public float spinDuration;
    public float maxSpinSpeed, stopDuration;
    public float maxSpinSpeedDelay;
    [Header("Slam")]
    public float slamDuration;
    [Header("Meteor Hurl")]
    public int meteorAmount;
    public float meteorRiseInterval,
                 meteorRiseDuration,
                 meteorFallDuration;

    public float RandomAttackTime {
        get {
            return Random.Range(attackTimeRange.x,
                                attackTimeRange.y);
        }
    }
}