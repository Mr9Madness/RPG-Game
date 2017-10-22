using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour {

    public int TargetFPS = 100;

	// Use this for initialization
    void Awake() {
        DontDestroyOnLoad( this );

        Application.targetFrameRate = TargetFPS;
    }
    
}
