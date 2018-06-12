using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.UI.Keyboard;
public class keyboardAppear : MonoBehaviour {
    
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void startKeyboard() {
        Keyboard.Instance.PresentKeyboard();
    }
}
