using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HourglassController : MonoBehaviour
{
    #region Properties
    [Header("Sands")]
    [SerializeField] HourglassSand SandTop;
    [SerializeField] HourglassSand SandBot;

    [Space] [Header("Controls")]
    // Controls the fill amount between the top and bottom halves
    [Range(0f, 1f)]
    [SerializeField] float fill = 0f;

    [SerializeField] Vector2 topPaddingMinMax = new Vector2(0, 0.05f);
    [SerializeField] Vector2 botPaddingMinMax = new Vector2(0.05f, 0.05f);
    
    #endregion
    
    void Start()
    {
        if (SandTop != null && SandBot != null)
        {
            FillSand();
        }
    }
    
    void FillSand()
    {
        SandTop.fill = 1 - fill;
        SandBot.fill = fill;

        SandTop.shapePadding = Mathf.Lerp(topPaddingMinMax.x, topPaddingMinMax.y, fill);
        SandBot.shapePadding = Mathf.Lerp(botPaddingMinMax.x, botPaddingMinMax.y, fill);
    }

    public void SetFill(float fill)
    {
        if (SandTop != null && SandBot != null) {
            this.fill = fill;
            FillSand();
        }
    }
}
