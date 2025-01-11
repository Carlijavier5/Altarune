using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    [SerializeField] private Button startButton, creditsButton, exitButton;
    [SerializeField] private SceneRef labScene, creditsScene;

    void Awake() {
        startButton.onClick.AddListener(StartGame);
        creditsButton.onClick.AddListener(ShowCredits);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void StartGame() {
        SceneManager.LoadScene(labScene.BuildIndex);
    }

    private void ShowCredits() {
        SceneManager.LoadScene(creditsScene.BuildIndex);
    }

    private void ExitGame() {
        Application.Quit();
    }
}