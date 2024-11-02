using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    private Image _img;

    void Awake() {
        _img = transform.GetComponent<Image>();
    }

    void Update() {
        _img.fillAmount = SceneLoader.Instance.GetLoadingProcess();
    }
}
