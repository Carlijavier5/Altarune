using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointStorage : MonoBehaviour
{
    private static CheckpointStorage instance;
    public static CheckpointStorage Instance{ get {return instance;}}
    private ArrayList checkpoints;

    private void Awake(){
        checkpoints = new ArrayList();
        //checkpoints.Add("Inner Sanctum Entrance");
        //checkpoints.Add("Inner Sanctum Entrance");
        //checkpoints.Add("Central Processing Unity");
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    public ArrayList getCheckpoints(){
        return checkpoints;
    }
    public void addCheckpoint(String text){
        checkpoints.Add(text);
    }
}
