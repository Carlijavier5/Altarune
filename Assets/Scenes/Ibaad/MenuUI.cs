using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button howToPlayButton;

    [SerializeField] private Button exitButton;

    private void Awake() {
        //START BUTTON
        startButton.onClick.AddListener(() => {
            SceneManager.LoadScene(1);
        });
        //OPTIONS BUTTON
        optionsButton.onClick.AddListener(() => {
        });
        //EXIT BUTTON
        exitButton.onClick.AddListener(() => {
            Debug.Log("Game Exited");
            Application.Quit();
        });
    }
}
