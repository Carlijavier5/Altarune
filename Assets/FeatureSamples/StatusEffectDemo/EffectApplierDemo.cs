using UnityEngine;

/// <summary>
/// Demo effect applier, which needs not be an entity, a summon, or even a base object!
/// </summary>
public class DemoEffectApplier : MonoBehaviour {

    [SerializeField] private DemoStatusEffect demoEffect;

    void OnTriggerEnter(Collider other) {
        /// On contact with an entity apply the effect;
        if (other.TryGetComponent(out Entity entity)) {

            entity.ApplyEffects(new[] { demoEffect.Clone() });

            /// ApplyEffects takes in an array, the code above, 'new[] { demoEffect.Clone() }'
            /// is just a explicit declaration of an array with a single element;

            /// If you have a serialized copy of the effect (the kind that you view in the inspector),
            /// make sure you pass a CLONE into the method, not an alias!

            /// Note that you can also instantiate effects directly if you don't need any inspector setup;
            /// Status effects are C# objects, so the following statement is legal:
            /// 
            /// StatusEffect someEffect = new();
        }
    }
}

/// <summary>
/// All effects inherit from status effect;
/// Note that this setup is exclusive to Altarune;
/// </summary>
[System.Serializable]
public class DemoStatusEffect : StatusEffect {

    /// <summary> Exposed modifiers in the inspectors, for ease of customization; </summary>
    [SerializeField] private HealthAttributeModifiers demoModifiers;

    [SerializeField] private Material material;
    [SerializeField] private float maxDuration;
    private float timer;

    /// <summary>
    /// Apply() gets called when the effect is initialized. Think of it as the effect's Awake/Start method;
    /// </summary>
    /// <param name="entity"> Entity that received the effect; </param>
    /// <param name="isNew"> Whether the effect is new or was already applied; </param>
    public override void Apply(Entity entity, bool isNew) {
        if (isNew) {
            entity.SetMaterial(material);   /// Changes the material of the target entity (see BaseObject.cs);
            AttributeMods = demoModifiers;  /// Apply modifiers for the effect when the effect is initialized (see StatusEffect.cs);
        } else {
            timer = 0;                      /// Reset the effect timer when the effect is reapplied;
        }
    }

    /// <summary>
    /// Update() gets called every frame on the entity that received the effect;
    /// </summary>
    /// <returns> Return true to Terminate the effect;<br/>
    /// Do not call Terminate() within the effect, since it must be removed on the Entity's side; </returns>
    public override bool Update(Entity entity) {

        /// Updates the timer every frame and prints a message every second;
        /// Removes the effect when the max duration is reached;

        timer += Time.deltaTime;
        if (timer % 1 == 0) Debug.Log($"Effect Timer: {timer} sec(s)");
        return timer >= maxDuration;
    }

    /// <summary>
    /// Terminate() gets called before the effect is removed; <br/>
    /// Note that this removal can be prompted by Entity death, not just by the effect itself;
    /// </summary>
    /// <param name="entity"></param>
    public override void Terminate(Entity entity) {
        entity.ResetMaterials();        /// Resets the materials on the host entity (see BaseObject.cs);
    }

    /// Note that this is a basic example! There's plenty you can do with these; <br/>
    /// You could spawn objects and keep track of them within a status effect, <br/>
    /// call any method on the host entity (i.e. TryDamage(), TryPush(), etc), <br/>
    /// and run any logic you'd like! DM @che0gorath for any questions o7
}