using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;

public class targetClicker : MonoBehaviour {
    timeManager time = new timeManager();
    public GameObject centerButton;
    GameObject[] targetSet;
    GameObject myTarget;
    float myTime = 0;
    int numofTargetClicked = 0;
    int index;
    int currentTargetIndex;

    // Use this for initialization
    void Start () {
        targetSet = GameObject.FindGameObjectsWithTag("target");
        for (int x = 0; x< targetSet.Length; x++) {
            targetSet[x].SetActive(false);
        }
        Debug.Log(targetSet.Length);
    }

    public GameObject arrow;
    public float timer = 0f;
    public float trialTimeout = 5f;
    
	// Update is called once per frame
	void Update () {
        if (arrow.activeSelf) {
            timer += Time.deltaTime;
            if(timer > trialTimeout) {
                arrow.SetActive(false);
            }
        }
	}

    public void targetOnClick () {
        Debug.Log("isClicked");
        Debug.Log(gameObject.name);
        
        gameObject.SetActive(false);
        //centerButton.SetActive(true);
        UserStudyOneScript.currentOrder ++;
    }

    public void centerTargetClicked() {
        Debug.Log("CenterClicked");

        // time.pressRefine();
        /* index = Random.Range(0, setTarget.Length);
         myTarget = setTarget[index];
         if (numofTargetClicked < setTarget.Length) {
             while (myTarget.tag == "Untagged") {
                 index = Random.Range(0, setTarget.Length);
                 myTarget = setTarget[index];
             }
         }
         else {
             for (int x = 0; x < setTarget.Length; x++) {
                 setTarget[x].tag = "target";
             }
             numofTargetClicked = 0;
             return;
         }
         myTarget.tag = "Untagged";
         myTarget.SetActive(true);
         centerButton.SetActive(false);

         */
        // draw a line from center to the first target;
        myTarget = targetSet[currentTargetIndex];
        arrow.SetActive(true);
        Vector3 direction = myTarget.transform.position - centerButton.transform.position;
        arrow.transform.RotateAround(arrow.transform.position, Vector3.forward, Vector3.SignedAngle(Vector3.up, direction, Vector3.forward));

        //numofTargetClicked++;
    }

    public void setTargetActive(int targetIndex) {
        Debug.Log("set target " + targetIndex.ToString() + " active");
        // assign i to current target
        currentTargetIndex = targetIndex;
        centerButton.SetActive(true);
        if (targetIndex >=0 && targetIndex<=7) {
            centerButton.GetComponent<Renderer>().material.color = Color.green;
        }
        if (targetIndex >= 8 && targetIndex <= 15) {
            centerButton.GetComponent<Renderer>().material.color = Color.yellow;
        }
        if (targetIndex >= 16 && targetIndex <= 23) {
            centerButton.GetComponent<Renderer>().material.color = Color.red;
        }
    }
}
