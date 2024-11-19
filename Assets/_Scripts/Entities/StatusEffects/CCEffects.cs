using UnityEngine;

[System.Serializable]
public class CrowdControlEffects {
    public CrowdControlNode stun;
    public CrowdControlNode root;
    public SlowCCNode slow;

    public void Update(float deltaTime) {
        stun?.Update(deltaTime);
        root?.Update(deltaTime);
        slow?.Update(deltaTime);
    }
}

[System.Serializable]
public class CrowdControlNode {
    public float duration;
    public bool pierceRes;

    public bool IsActive => duration > 0;

    public void Update(float deltaTime) {
        duration -= deltaTime;
    }
}

[System.Serializable]
public class SlowCCNode : CrowdControlNode {
    [SerializeField] private float multiplier;
    public float Multiplier {
        get => multiplier;
        set => multiplier = Mathf.Clamp(value, 0.1f, 10f);
    }
}