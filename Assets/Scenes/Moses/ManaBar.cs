using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{

    
    // UI Elements - Try To Implement With Horizontal Bar issues with Half Radial Circle
    public Image horizontalMana; // Horizontal


    public Image verticalMana_o; // Vertical Out
    public Image verticalMana_t; // Vertical Top
    public Image radialCircleMana; // Full Circle
    public Image radialHalfMana;  // Semi Circle

  

    // Current Mana
    private float currMana = 20f;
    private float maxMana = 20f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize with a specific value or total
        currMana = maxMana; // Starting with horizontal as an example
        manaBarFiller(); // Initialize the UI at the start
    }

    // Method to adjust current mana and update UI
    public void SetCurrentMana(float newMana)
    {
        currMana = Mathf.Clamp(newMana, 0, maxMana); // Clamp for horizontal as default
        manaBarFiller(); // Update the UI after changing mana
    }

    // Update UI elements based on current mana
    void manaBarFiller()
    {
        // Calculate the mana percentage for each UI type
        float horizontalPercentage = currMana / maxMana;
        float verticalPercentage = currMana / maxMana;
        float radialCirclePercentage = currMana / maxMana;
        float radialHalfPercentage = currMana / maxMana;

        // Update the fill amount for each UI element
        horizontalMana.fillAmount = horizontalPercentage;
        verticalMana_o.fillAmount = verticalPercentage;
        verticalMana_t.fillAmount = verticalPercentage;
        radialCircleMana.fillAmount = radialCirclePercentage;
        radialHalfMana.fillAmount = radialHalfPercentage;
    }

    // Coroutine to smoothly transition the mana fill

    // I wachted a Tutorial on Tweening The Instant Mana Fill Looked so Ugly and static
    private IEnumerator TweenManaFill(Image manaImage, float targetFill, float duration)
    {
        float startFill = manaImage.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Normalize the time
            manaImage.fillAmount = Mathf.Lerp(startFill, targetFill, t); // Lerp fill amount
            yield return null; // Wait for the next frame
        }

        manaImage.fillAmount = targetFill; // Ensure the fill amount is set to the target at the end
    }
    public void enactSpellConsumeMana(float manaCost)
    {
        currMana -= manaCost;
        currMana = Mathf.Clamp(currMana, 0, maxMana); // Ensure mana does not go below 0

        StartCoroutine(TweenManaFill(horizontalMana, currMana/ maxMana, 1f)); // Tween for horizontal

        // Carlos I tried changing the  targetFill value to currMana / (maxMana * 0.5f). However the radial stopped working after that
        StartCoroutine(TweenManaFill(radialHalfMana, currMana / (maxMana), 1f));
        Debug.Log("Mana Spent was 5");
    }

    public bool PrepareSpell(float manaCost)
    {
        string spellName = "RandomSpell";
        float spellCost = 5f;

        // Check if the spell can be cast
        if (currMana >= spellCost)
        {
            Debug.Log("Can Cast The Spell");
            return true;
        }
        else
        {
            Debug.Log("No Mana Cannot Cast");
            return false;
        }
    }


    // Example method to restore mana
    public void RestoreMana()
    {
        currMana = maxMana;
        StartCoroutine(TweenManaFill(horizontalMana, currMana / maxMana, 1f)); // Tween for horizontal
        StartCoroutine(TweenManaFill(radialHalfMana, currMana / maxMana, 1f));
        SetCurrentMana(currMana); // Update the current mana
    }
}
