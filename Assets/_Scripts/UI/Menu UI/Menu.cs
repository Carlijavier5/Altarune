using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Menu : MonoBehaviour {

    public UnityEvent opened, closed;

    [SerializeField] private Menu backMenu;
    [SerializeField] private bool openOnStart;
    [SerializeField] private float closeSpeed = 0.2f,
                                   openSpeed = 0.1f;
    public bool IsOpen { get; private set; }
    private Coroutine currentMenuAction;

    void Awake() {
        if (openOnStart) {
            gameObject.SetActive(true);
            OpenMenu();
        } else {
            gameObject.SetActive(false);
        }
    }

    public void OpenAsNextMenu() {
        if (backMenu.IsOpen) {
            backMenu.closed.AddListener(OpenAsNextMenu);
            backMenu.CloseMenu();
            return;
        }

        backMenu.closed.RemoveListener(OpenAsNextMenu);
        OpenMenu();
    }

    public void BackMenu() {
        if (backMenu == null) return;

        if (IsOpen) {
            CloseMenu();
            closed.AddListener(BackMenu);
            return;
        }

        closed.RemoveListener(BackMenu);
        backMenu.OpenMenu();
    }

    public void CloseMenu() {
        if (currentMenuAction != null) {
            StopCoroutine(currentMenuAction);
            currentMenuAction = null;
        }

        gameObject.SetActive(true);
        currentMenuAction = StartCoroutine(CloseMenuCoroutine());
    }

    public void OpenMenu() {
        if (currentMenuAction != null) {
            StopCoroutine(currentMenuAction);
            currentMenuAction = null;
        }

        gameObject.SetActive(true);
        currentMenuAction = StartCoroutine(IOpenMenu());

    }

    private IEnumerator IOpenMenu() {
        gameObject.SetActive(true);
        float time = 0;
        bool scalingUp = true;
        while (scalingUp) {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time / openSpeed);

            if (time >= openSpeed) scalingUp = false;
            time += Time.deltaTime;
            yield return null;
        }

        IsOpen = true;
        opened.Invoke();
    }

    IEnumerator CloseMenuCoroutine() {
        bool scalingDown = true;

        float time = Mathf.Epsilon;
        while (scalingDown) {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Mathf.Min(time / closeSpeed, 1));

            if (time >= closeSpeed) scalingDown = false;
            time += Time.deltaTime;
            yield return null;
        }

        IsOpen = false;
        closed.Invoke();
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
}