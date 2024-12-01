using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueCharacterController : MonoBehaviour {

    public System.Action<DialogueCharacterController> OnFadeInEnd;

    [SerializeField] private DialoguePortrait portrait;
    [SerializeField] private RectTransform titleRect;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private float anchorSpeed;

    private Vector2 nameYAnchor;
    private Vector2 targetAnchor = new(0.265f, 0.34f);

    void Awake() {
        titleRect.gameObject.SetActive(false);
        nameYAnchor = new Vector2(titleRect.anchorMin.y, titleRect.anchorMax.y);
    }

    public void UpdateCharacter(CharacterData data, bool active) {
        portrait.Materialize(data != null);
        if (data) {
            portrait.AssignSprite(data.sprite);
            titleText.text = data.titleName;
        } ToggleCharacter(active && data);
    }

    public void ToggleCharacter(bool active) {
        portrait.Toggle(active);
        StopAllCoroutines();
        if (active) StartCoroutine(IShowTitle());
        else StartCoroutine(IHideTitle());
    }

    private IEnumerator IShowTitle() {
        titleRect.gameObject.SetActive(true);
        while (titleRect.anchorMin.y != targetAnchor.x
               || titleRect.anchorMax.y != targetAnchor.y) {
            titleRect.anchorMin = Vector2.MoveTowards(titleRect.anchorMin, new Vector2(titleRect.anchorMin.x, targetAnchor.x),
                                                      Time.unscaledDeltaTime * anchorSpeed);
            titleRect.anchorMax = Vector2.MoveTowards(titleRect.anchorMax, new Vector2(titleRect.anchorMax.x, targetAnchor.y),
                                                      Time.unscaledDeltaTime * anchorSpeed);
            yield return null;
        }
    }

    private IEnumerator IHideTitle() {
        while (titleRect.anchorMin.y != nameYAnchor.x
               || titleRect.anchorMax.y != nameYAnchor.y) {
            titleRect.anchorMin = Vector2.MoveTowards(titleRect.anchorMin, new Vector2(titleRect.anchorMin.x, nameYAnchor.x),
                                                      Time.unscaledDeltaTime * anchorSpeed * 2);
            titleRect.anchorMax = Vector2.MoveTowards(titleRect.anchorMax, new Vector2(titleRect.anchorMax.x, nameYAnchor.y),
                                                      Time.unscaledDeltaTime * anchorSpeed * 2);
            yield return null;
        } titleRect.gameObject.SetActive(false);
    }
}