using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ManaCellStatus { Standard, Commit, Insufficient }

public class ManaCellActivationOverlay : MonoBehaviour
{
    [SerializeField] private PingPongFadeAnimator fadeAnimator;
    [System.Serializable] private class ColorGroup { public Color center, corners, fill; }
    [SerializeField] private ColorGroup standardColors, commitColors, insufficientColors;
    [SerializeField] private Image centerImage, cornerImage, fillImage;
    [SerializeField] private float tintSpeed;

    public void SetTint(ManaCellStatus status) {
        ColorGroup tintGroup = status switch { ManaCellStatus.Commit => commitColors,
                                               ManaCellStatus.Insufficient => insufficientColors,
                                               _ => standardColors };
        StopAllCoroutines();
        StartCoroutine(IDoTint(tintGroup));
        fadeAnimator.DoFade(status != ManaCellStatus.Standard);
    }

    private IEnumerator IDoTint(ColorGroup tintGroup) {
        while (ApproachTint(centerImage, tintGroup.center)
                || ApproachTint(cornerImage, tintGroup.corners)
                    || ApproachTint(fillImage, tintGroup.fill)) {
            yield return null;
        }
    }

    private bool ApproachTint(Image image, Color targetColor) {
        image.color = Vector4.MoveTowards(image.color, targetColor, Time.unscaledDeltaTime * tintSpeed);
        return Vector4.Distance(image.color, targetColor) > 0;
    }
}