using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeleportMenu : MonoBehaviour
{
    //[SerializeField] private string[] nexusConnections;
    
    private ArrayList nexusConnections;

    [SerializeField] private NexusButton buttonPrefab;
    [SerializeField] private TextMeshProUGUI emptyString;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform anchor;
    private NexusButton[] nexusButtons;
    // Start is called before the first frame update
    void Start() {
        //nexusConnections = LevelStateManager.Instance.getCheckpoints();
        nexusButtons = new NexusButton[nexusConnections.Count];
        RectTransform rectTransform = buttonPrefab.GetRectTransform();
        float maxHeight = rectTransform.rect.height * 1.3f * (nexusConnections.Count - 1) + 1.5f;
        for (int i = 0;i < nexusConnections.Count;i++){
            nexusButtons[i] = Instantiate(buttonPrefab);
            nexusButtons[i].transform.SetParent(anchor.transform);
            Vector3 position = new Vector3(0f, 0f, 0f);
            position.x = rectTransform.rect.width / 2f;
            position.y = rectTransform.rect.height * 1.3f * (nexusConnections.Count - 1 - i) + 1.5f;
            nexusButtons[i].SetStart(new Vector3(0f, maxHeight, 0f));
            nexusButtons[i].setText((String)nexusConnections[i]);
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
