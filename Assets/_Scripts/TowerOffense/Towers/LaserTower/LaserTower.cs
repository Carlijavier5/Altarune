using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaserTower : Summon {

	[SerializeField] private LaserTowerProjectile towerProjectile;
	[SerializeField] private Transform muzzle;
	[SerializeField] private float attackInterval;
	[SerializeField] private float checkClosestEnemyInterval;
	//This prefab needs to have the enemy factions set to include enemies
	[SerializeField] private AggroRange attackRangePrefab;
	private bool init;
	private float attackTick = 0.2f, checkClosestEnemyTick = 0.2f;
	private AggroRange attackRange;
	private Entity closestEnemy;

	protected override void Awake()
	{
		base.Awake();
	}

	public override void Init() {
		attackRange = Instantiate<AggroRange>(attackRangePrefab, gameObject.transform);
		
		init = true;
	}

	void Update() {
		if (!init) return;

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
		if (attackTick >= attackInterval && closestEnemy != null) {
			Debug.Log("Firing!");
			Instantiate(towerProjectile, muzzle.transform.position, Quaternion.LookRotation(closestEnemy.transform.position - gameObject.transform.position));

			attackTick = 0;
		} else if (attackTick >= attackInterval) {
			Debug.Log("No closest enemy to fire at");
		}
	}
}