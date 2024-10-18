using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// For loading into a different scene
/// </summary>
public static class SceneLoader
{
    // dummy class to run coroutine
    private class LoadingMonoBehavior : MonoBehaviour { }

    public enum Scene {
        M2_MainMenu,
        M2_Game,
        M2_Lab,
        M2_Loading,
    }

    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scene scene) {
        // set the loader callback action to load the target scene
        onLoaderCallback = () => {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehavior>().StartCoroutine(LoadSceneAsync(scene));
        };

        // load the loading scene
        SceneManager.LoadScene(Scene.M2_Loading.ToString());
    }

    private static IEnumerator LoadSceneAsync(Scene scene) {
        yield return null;

        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!loadingAsyncOperation.isDone) {
            yield return null;
        }
    }

    public static float GetLoadingProcess() {
        if (loadingAsyncOperation != null) { return loadingAsyncOperation.progress; }
        else { return 1f; }
    }

    public static void LoaderCallback() {
        // triggered after the first update which lets the screen refresh
        // execute the loader callback action which will load the target scene
        if (onLoaderCallback != null) {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
