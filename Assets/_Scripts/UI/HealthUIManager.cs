using System.Collections.Generic;
using UnityEngine;

public class HealthUIManager : MonoBehaviour {

    [SerializeField] private Player player;
    [SerializeField] private HealthSpriteController[] healthSprites;

    void Awake() {
        player.InputSource.OnPlayerInit += InputSource_OnPlayerInit;
        player.OnDamageReceived += Player_OnDamageReceived;
        player.OnHealReceived += Player_OnHealReceived;
    }

    private void InputSource_OnPlayerInit() {
        int health = player.Health;
        for (int i = 0; i < healthSprites.Length; i++) {
            healthSprites[i].gameObject.SetActive(health > i);
        }
    }

    private void Player_OnDamageReceived(int _) {
        int currHealth = player.Health;
        int maxHealth = player.MaxHealth;
        for (int i = currHealth; i < maxHealth; i++) {
            healthSprites[i].Break();
        }
    }

    private void Player_OnHealReceived(int _) {
        int currHealth = player.Health;
        for (int i = 0; i < currHealth; i++) {
            healthSprites[i].Restore();
        }
    }
}