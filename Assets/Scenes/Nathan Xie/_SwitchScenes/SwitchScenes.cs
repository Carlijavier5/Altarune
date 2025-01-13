using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenes : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)){
            SceneManager.LoadScene(sceneName:"M2_Lab");
        } else if(Input.GetKeyDown(KeyCode.B)){
            SceneManager.LoadScene(sceneName:"Scene2");
        }
    }
}
