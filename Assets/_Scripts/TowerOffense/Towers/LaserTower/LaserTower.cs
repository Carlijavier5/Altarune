using Unity.VisualScripting;
using UnityEngine;

public class LaserTower : Summon {

	[SerializeField] private LaserTowerProjectile towerProjectile;
	[SerializeField] private AltLaserTowerBeam altTowerProjectile;
	[SerializeField] private Transform muzzle;
	[SerializeField] private float attackInterval, altAttackCooldown;
	[SerializeField] private float checkClosestEnemyInterval;
	[SerializeField] private float range = -1, altAttackRange = -1;
	//This prefab needs to have the enemy factions set to include enemies
	[SerializeField] private AggroRange attackRange;
	private bool altAttackMode = false;
	private float attackTick = 0.2f, checkClosestEnemyTick = 0.2f;
	// TODO: Fix Layermask
	private int environmentLayerMask = ~(1 << 6), enemyAndEnvironmentLayerMask = ~((1 << 6) ^ 3); //Environment Mask for checking collisions
	private Entity closestEnemy, altAttackTarget;
	private AltLaserTowerBeam altAttackBeam;

	[SerializeField] private LaserTowerAnimator animator;


	public override void Init(SummonData data, Entity summoner,
						      ManaSource manaSource) {
		base.Init(data, summoner, manaSource);
	}

	private void ClearAltAttackBeam() {
		altAttackBeam = null;
	}

	void Update() {
		if (!active) return;
		checkClosestEnemyTick += Time.deltaTime;
		if (checkClosestEnemyTick >= checkClosestEnemyInterval) {
			float range = closestEnemy != null ? Vector3.Distance(gameObject.transform.position, closestEnemy.transform.position) : -1;

			if (attackRange.AggroTargets.Count != 0) {
				foreach (Entity e in attackRange.AggroTargets) {
					if (closestEnemy == null || e.GetHashCode() != closestEnemy.GetHashCode()) {
						float d = Vector3.Distance(gameObject.transform.position, e.transform.position);
						if (range == -1 || d < range) {
							closestEnemy = e;
							range = d;
						}
					}
				}
			} else {
				closestEnemy = null;
			}
			
		}

		attackTick += Time.deltaTime;
		if (!altAttackMode) {
			if (attackTick >= attackInterval && closestEnemy != null) {
				bool hit = Physics.Raycast(muzzle.transform.position, Quaternion.LookRotation(closestEnemy.transform.position - gameObject.transform.position) * Vector3.forward, out RaycastHit raycastHit, range < 0 ? Mathf.Infinity : range, environmentLayerMask, QueryTriggerInteraction.Ignore);
				
				UnityEngine.Object laserProjectile = Instantiate(towerProjectile, muzzle.transform.position, Quaternion.LookRotation(closestEnemy.transform.position - gameObject.transform.position));
				laserProjectile.GetComponent<LaserTowerProjectile>().setOGscale(raycastHit.distance);
				animator.PlayLaser(Quaternion.LookRotation(closestEnemy.transform.position - gameObject.transform.position));

				attackTick = 0;
			}
		} else {
			if (attackTick >= altAttackCooldown) {
				if (altAttackTarget == null || Vector3.Distance(altAttackTarget.gameObject.transform.position, gameObject.transform.position) > altAttackRange) {
					altAttackTarget = closestEnemy;
				}

				if (altAttackTarget != null && altAttackBeam == null && Physics.Raycast(muzzle.transform.position, Quaternion.LookRotation(altAttackTarget.transform.position - gameObject.transform.position) * Vector3.forward, out RaycastHit raycastHit, range < 0 ? Vector3.Distance(altAttackTarget.transform.position, gameObject.transform.position) : range, enemyAndEnvironmentLayerMask, QueryTriggerInteraction.Ignore)) {
					if (raycastHit.collider.gameObject.layer != 6) {
						var laserProjectile = Instantiate(altTowerProjectile, muzzle.transform.position, Quaternion.LookRotation(closestEnemy.transform.position - gameObject.transform.position));
						animator.PlayLaser(Quaternion.LookRotation(closestEnemy.transform.position - gameObject.transform.position));
						altAttackBeam = laserProjectile.GetComponent<AltLaserTowerBeam>();
						altAttackBeam.giveData(altAttackRange < 0 ? 5f : altAttackRange, altAttackTarget, this.ClearAltAttackBeam);
						attackTick = 0;
					} else {
						altAttackTarget = null;
					}
				}
			} else if (altAttackBeam != null) {
				attackTick = 0;
			} else {
				attackTick += Time.deltaTime;
			}
		}
		
	}
}