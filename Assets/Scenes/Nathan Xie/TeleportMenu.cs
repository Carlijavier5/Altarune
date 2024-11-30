using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeleportMenu : MonoBehaviour
{
    [SerializeField] private string[] nexusConnections;
    [SerializeField] private NexusButton buttonPrefab;
    [SerializeField] private TextMeshProUGUI emptyString;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform anchor;
    private NexusButton[] nexusButtons;
    // Start is called before the first frame update
    void Start() {
        nexusButtons = new NexusButton[nexusConnections.Length];
        RectTransform rectTransform = buttonPrefab.GetRectTransform();
        float maxHeight = rectTransform.rect.height * 1.3f * (nexusConnections.Length - 1) + 1.5f;
        for (int i = 0;i < nexusConnections.Length;i++){
            nexusButtons[i] = Instantiate(buttonPrefab);
            nexusButtons[i].transform.SetParent(anchor.transform);
            Vector3 position = new Vector3(0f, 0f, 0f);
            position.x = rectTransform.rect.width / 2f;
            position.y = rectTransform.rect.height * 1.3f * (nexusConnections.Length - 1 - i) + 1.5f;
            nexusButtons[i].SetStart(new Vector3(0f, maxHeight, 0f));
            nexusButtons[i].setText(nexusConnections[i]);
            nexusButtons[i].SetDestination(position);
            NexusButton button = nexusButtons[i];
            nexusButtons[i].getButton().onClick.AddListener(() => ButtonClick(button));
        }
        if (nexusButtons.Length == 0) {
            emptyString.alpha = 1;
        }
    }
    private void ButtonClick(NexusButton buttonPressed){
        Debug.Log(buttonPressed.getText() + " has been pressed");
    }

    private void StartButtonMovement(){
        for (int i = 0;i < nexusButtons.Length;i++) {
            nexusButtons[i].StartMovement();
        }
    }

    // Update is called once per frame
    void Update(){
        ShowButton();
    }

    private void ShowButton(){
        if(Input.GetKeyDown(KeyCode.U)){
            if(canvasGroup.interactable) {
                canvasGroup.alpha = 0;
            } else {
                StartButtonMovement();
                canvasGroup.alpha = 1;
            }
            canvasGroup.interactable = !canvasGroup.interactable;

        }
    }
}
