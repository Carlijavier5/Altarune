using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InversionLoader : MonoBehaviour {

    public event System.Action OnInversionEnd;

    public abstract void InvertStance();
}
