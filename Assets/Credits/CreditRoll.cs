using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CreditRoll : ScriptableObject {

    [SerializeField] private CreditData[] credits;
    public CreditData[] Credits => credits.Clone() as CreditData[];
}

[System.Serializable]
public class CreditData {
    public int fontSizeIncrease;
    public string name, role;
    [TextArea] public string quote;
    public Sprite sprite;
}