using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    [SerializeField] private Button startButton, creditsButton, exitButton;
    [SerializeField] private SceneRef labScene, creditsScene;
    [SerializeField] private float minLoadTime;

    void Awake() {
        startButton.onClick.AddListener(StartGame);
        creditsButton.onClick.AddListener(ShowCredits);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void StartGame() {
        StartCoroutine(ILoadLevelAsync(labScene));
    }

    private void ShowCredits() {
        StartCoroutine(ILoadLevelAsync(creditsScene));
    }

    private void ExitGame() {
        Application.Quit();
    }

    private IEnumerator ILoadLevelAsync(SceneRef scene) {
        yield return new WaitForSeconds(1);
        AsyncOperation op = SceneManager.LoadSceneAsync(scene.BuildIndex);
        float timer = 0;
        while (timer < minLoadTime || !op.isDone) {
            timer += Time.deltaTime;
            yield return null;
        }
    }
}