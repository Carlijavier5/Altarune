using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour {

    protected float timeScale = 1;
    public virtual float TimeScale { get => timeScale; set => timeScale = value; }

    protected float DeltaTime => Time.deltaTime * timeScale;
}
