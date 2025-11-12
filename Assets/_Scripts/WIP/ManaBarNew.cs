using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarNew : MonoBehaviour {
    [SerializeField] private ManaSource manaSource;
    [SerializeField] private ManaCellManager cellManager;
    [SerializeField] private Image topLayer, topLayerFX, interpolationLayer;
    [SerializeField] private ParticleSystem fillParticleSystem;

    private float Mana => manaSource.Mana;
    private float MaxMana => manaSource.MaxMana;
    private float FillAmount => MaxMana > 0 ? Mana / MaxMana : 0;
    private float TopLayerFill {
        set {
            topLayer.fillAmount = value;
            topLayerFX.fillAmount = value;
        }
    }

    void Start() {
        cellManager.Init(manaSource.MaxCells, manaSource.FullCells);
        manaSource.OnManaFill += ManaSource_OnManaFillNoTrigger;
        manaSource.OnManaDrain += ManaSource_OnManaDrain;
    }

    void Update() {
        TopLayerFill = Mathf.MoveTowards(topLayer.fillAmount,
                                         FillAmount, Time.deltaTime);
        interpolationLayer.fillAmount = Mathf.MoveTowards(interpolationLayer.fillAmount,
                                                          FillAmount, Time.deltaTime);

        RectTransform rt = interpolationLayer.rectTransform;
        float localRightX = rt.rect.xMin + rt.rect.width * interpolationLayer.fillAmount;
        Vector3 worldRightEdge = rt.TransformPoint(new Vector3(localRightX, 0, 0));

        fillParticleSystem.transform.position = worldRightEdge;
    }

    private void ManaSource_OnManaFill(int cellsGained) {
        cellManager.ActivateCells(cellsGained);
        interpolationLayer.fillAmount = Mana / MaxMana;
    }

    private void ManaSource_OnManaFillNoTrigger(int cellsGained) {
        cellManager.ActivateCells(cellsGained);
    }

    private void ManaSource_OnManaDrain(int cellsLost) {
        cellManager.DeactivateCells(cellsLost);
        TopLayerFill = Mana / MaxMana;
    }
}
