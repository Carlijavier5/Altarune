using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class Menu : MonoBehaviour
{

    public void Awake()
    {
        if (openOnStart)
        {
            gameObject.SetActive(true);
            OpenMenu();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void OnCancel()
    {
        if (open)
        {
            BackMenu();
        }
    }

    public void OpenAsNextMenu()
    {
        if (backMenu.open)
        {
            backMenu.closed.AddListener(OpenAsNextMenu);
            backMenu.CloseMenu();
            return;
        }
        backMenu.closed.RemoveListener(OpenAsNextMenu);
        OpenMenu();

    }

    public void BackMenu()
    {
        if (backMenu == null) return;

        if (open) {
            CloseMenu();
            closed.AddListener(BackMenu);
            return;
        }
        closed.RemoveListener(BackMenu);
        backMenu.OpenMenu();

    }


    public void CloseMenu()
    {
        if (currentMenuAction != null)
        {
            StopCoroutine(currentMenuAction);
            currentMenuAction = null;
        }
        gameObject.SetActive(true);
        currentMenuAction = StartCoroutine(CloseMenuCoroutine());
    }

    public void OpenMenu()
    {
        if (currentMenuAction != null)
        {
            StopCoroutine(currentMenuAction);
            currentMenuAction = null;
        }
        gameObject.SetActive(true);
        currentMenuAction = StartCoroutine(OpenMenuCoroutine());

    }


    [SerializeField] Menu backMenu;
    [SerializeField] bool openOnStart;

    [SerializeField] bool lockXScale;
    [SerializeField] bool lockYScale;

    public bool open;

    Coroutine currentMenuAction;

    public float closeSpeed = 0.2f;
    public float openSpeed = 0.1f;
    public UnityEvent opened;
    public UnityEvent closed;
    IEnumerator OpenMenuCoroutine()
    {
        gameObject.SetActive(true);
        float time = 0;
        bool scalingUp = true;
        while (scalingUp)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time / openSpeed);
            if (lockXScale) transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            if (lockYScale) transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);

            if (time >= openSpeed) scalingUp = false;
            time += Time.deltaTime;
            // Wait for next frame
            yield return new WaitForSeconds(0);
        }
        open = true;
        opened.Invoke();
    }

    IEnumerator CloseMenuCoroutine()
    {
        bool scalingDown = true;

        // i dont trust zero scale
        float time = 0.0001f;
        while (scalingDown)
        {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Mathf.Min(time / closeSpeed, 1));
            if (lockXScale) transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            if (lockYScale) transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);

            if (time >= closeSpeed) scalingDown = false;
            time += Time.deltaTime;
            // Wait for next frame
            yield return new WaitForSeconds(0);
        }
        open = false;
        closed.Invoke();
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
}
