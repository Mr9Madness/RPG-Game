using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour {

    public int TargetFPS = 120;

	// Use this for initialization
    void Awake() {
        DontDestroyOnLoad( this );

        Application.targetFrameRate = TargetFPS;
    }
    
}
