using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{

    // Carlos I made another class SpellCasting because of the issues with buttons and methods having to be void
    // I think My Implmentation works better this way.

    // Reference to the ManaBar to check and manage mana
    public ManaBar manaBar;
    public bool canCast = false;

    // Method to attempt to cast a spell
    public void PrepareSpell()
    {

        string spellName = "RandomSpell";

        float spellCost = 5f;

        Debug.Log($"Preparing to cast {spellName}, which costs {spellCost} mana.");

        // Check if we have enough mana
        if (manaBar.PrepareSpell(spellCost))
        {
            canCast = true;
        }
        
    }

    // This Methos is used to cast the Spell
    public void CastSpell()
    {
        string spellName = "RandomSpell";
        float spellCost = 5f;
        if (canCast)
        {
            Debug.Log($"{spellName} spell is being cast!");
            // Call the ManaBar to consume the mana
            manaBar.enactSpellConsumeMana(spellCost);
        }
    }

    // Method to cancel the spell if necessary
    public void CancelSpell()
    {
        Debug.Log("Spell casting has been cancelled.");
        // I haven't impemented this cancel logic yet
    }
}
