using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour {
    [SerializeField] private int health = 10;
    [SerializeField] private GameObject healthSprite;
    [SerializeField] private float spacing = 2f;
    [SerializeField] private Transform healthGroup;

    private List<GameObject> healthSprites = new List<GameObject>();

    private int currHealth;
    // Start is called before the first frame update
    void Awake() {
        currHealth = health;
        for (int i = 0; i < health; i++) {
            healthSprites.Add(Instantiate(healthSprite, new Vector3(i * spacing, 0f, 0f) + healthGroup.position,
                Quaternion.identity, healthGroup));
        }
    }

    /// <summary>
    /// Updates health visuals. Pass current health.
    /// </summary>
    /// <param name="currHealth">Current health</param>
    public void UpdateHealth(int currHealth) {
        if (this.currHealth > currHealth) {
            healthSprites[currHealth].SetActive(false);
        }
        else if (this.currHealth < currHealth) {
            if (this.currHealth + 1 < healthSprites.Count) healthSprites[currHealth].SetActive(false);
        }
        this.currHealth = currHealth;
    }
}
