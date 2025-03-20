using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkReader : MonoBehaviour {

    [SerializeField] private Image[] images;

    void Awake() {
        if (AchievementManager.Instance) {
            if (AchievementManager.Instance._level1Reached) {
                images[0].gameObject.SetActive(true);
            }
            if (AchievementManager.Instance._windTowerPlaced) {
                images[1].gameObject.SetActive(true);
            }
            if (AchievementManager.Instance._sentintelGolemInterrupted) {
                images[2].gameObject.SetActive(true);
            }
            if (AchievementManager.Instance._scarabQueenSpawnCheck) {
                images[3].gameObject.SetActive(true);
            }
            if (AchievementManager.Instance._siftlingCheck) {
                images[4].gameObject.SetActive(true);
            }
            if (AchievementManager.Instance._savageGolemCheck) {
                images[5].gameObject.SetActive(true);
            }
            if (AchievementManager.Instance._savageGolemWithSniperCheck) {
                images[6].gameObject.SetActive(true);
            }
            if (AchievementManager.Instance._lightningOnlyCheck) {
                images[7].gameObject.SetActive(true);
            }
            if (AchievementManager.Instance._sovereignGolemDefeatCheck) {
                images[8].gameObject.SetActive(true);
            }
            if (AchievementManager.Instance._perfectRunCheck) {
                images[9].gameObject.SetActive(true);
            }
        }
    }
}
