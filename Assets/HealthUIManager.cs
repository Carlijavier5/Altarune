using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUIManager : MonoBehaviour {

    [SerializeField] private Player player;
    [SerializeField] private GameObject healthSprite;
    [SerializeField] private float spacing = 2f;
    [SerializeField] private Transform healthGroup;

    private GameObject[] healthSprites;

    void Awake() {
        player.InputSource.OnPlayerInit += InputSource_OnPlayerInit;
        player.OnDamageReceived += Player_OnDamageReceived;
    }

    private void InputSource_OnPlayerInit() {
        int health = player.Health;
        healthSprites = new GameObject[health];
        for (int i = 0; i < health; i++) {
            healthSprites[i] = Instantiate(healthSprite, new Vector3(i * spacing, 0f, 0f) + healthGroup.position,
                Quaternion.identity, healthGroup);
        }
    }

    /// <summary>
    /// Updates health visuals. Pass current health.
    /// </summary>
    private void Player_OnDamageReceived(int _) {
        int currHealth = player.Health;
        int maxHealth = player.MaxHealth;
        for (int i = 0; i < maxHealth; i++) {
            if (i >= currHealth) {
                healthSprites[i].SetActive(false);
            }
        }
    }
}
