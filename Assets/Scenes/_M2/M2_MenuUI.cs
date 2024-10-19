using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M2_MenuUI : MonoBehaviour
{

    [SerializeField] private Button _startBtn;


    void Awake() {
        _startBtn.onClick.AddListener(()  => SceneLoader.Instance.Load(SceneLoader.Scene.M2_Lab));
    }
}
