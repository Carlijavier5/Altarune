using UnityEngine;

public class EnergyController : MonoBehaviour {

    public static EnergyController Instance { get; private set; }

    public event System.Action<float> OnEnergyChange;
    private float energy;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public static float GetEnergy() {
        return Instance ? Instance.energy : 0; 
    }

    public static void ChangeEnergy(float energy) {
        if (Instance) {
            Instance.energy += energy;
            Instance.OnEnergyChange?.Invoke(energy);
        }
    }
}