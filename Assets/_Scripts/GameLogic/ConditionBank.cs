using UnityEngine;

public class ConditionBank : MonoBehaviour {

    [SerializeField] private bool hasSovereignCutscene;
    public bool HasSovereignCutscene => hasSovereignCutscene;

    public void ClearSovereignCutscene() => hasSovereignCutscene = true;
}