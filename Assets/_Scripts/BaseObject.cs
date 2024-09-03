using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The object that all interactable object inherit from in the game.
/// When object A interacts with object B you interact through this base object class.
/// 
/// EXAMPLE: if one object damages another you go through base class
/// </summary>
public class BaseObject : MonoBehaviour {

    protected float timeScale = 1;
    public virtual float TimeScale { get => timeScale; set => timeScale = value; }

    protected float DeltaTime => Time.deltaTime * timeScale;
}