using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButton : MonoBehaviour
{
    Button button;

    [Tooltip("Used for loading partially. waits for the menu to close then loads")]
    [SerializeField] Menu menu;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ScaleButton);
    }
    public void LoadMainLevel()
    {
        if (menu != null)
        {
            if (menu.IsOpen)
            {
                menu.closed.AddListener(LoadMainLevel);
            }
            else
            {
                menu.closed.RemoveListener(LoadMainLevel);
                SceneLoader.Instance.Load(SceneLoader.Scene.M2_Lab);
            }
            return;
        }
        SceneLoader.Instance.Load(SceneLoader.Scene.M2_Lab);
    }
    public void LoadMainMenu()
    {
        if (menu != null)
        {
            if (menu.IsOpen)
            {
                menu.closed.AddListener(LoadMainMenu);
            }
            else
            {
                menu.closed.RemoveListener(LoadMainMenu);
                SceneManager.LoadSceneAsync(0);
            }
            return;
        }
        SceneManager.LoadSceneAsync(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadLink(string link)
    {
        Application.OpenURL(link);
    }


    Coroutine buttonCoroutine;
    public void ScaleButton()
    {
        if (buttonCoroutine != null) { StopCoroutine(buttonCoroutine); }
        buttonCoroutine = StartCoroutine(ButtonAnimation());
    }

    public float buttonSizePressed = 0.5f;
    public float scaleDownSpeed = 0.2f;
    public float scaleUpSpeed = 0.1f;
    public UnityEvent StartPress;
    public UnityEvent pressFinished;
    IEnumerator ButtonAnimation()
    {
        StartPress.Invoke();
        bool scalingDown = true;
        float time = 0;
        while (scalingDown)
        {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * buttonSizePressed, Mathf.Min( time / scaleDownSpeed, 1));
            if (time >= scaleDownSpeed) scalingDown = false;
            time += Time.deltaTime;
            // Wait for next frame
            yield return new WaitForSeconds(0);
        }
        time = 0;
        bool scalingUp = true;
        while (scalingUp)
        {
            transform.localScale = Vector3.Lerp(Vector3.one * buttonSizePressed, Vector3.one, time / scaleUpSpeed);
            if (time >= scaleUpSpeed) scalingUp = false;
            time += Time.deltaTime;
            // Wait for next frame
            yield return new WaitForSeconds(0);
        }
        pressFinished.Invoke();
    }
}
