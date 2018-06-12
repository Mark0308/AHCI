using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class timeManager : MonoBehaviour {

    public static timeManager myInstance;
    public float myTime;
    public float refineTime;
    public float selectTime;
    int numTrial = 10;
    bool starter;
    static int idNum = userIDinfo.myID;

    List<float> refineArray = new List<float>();
    List<float> selectArray = new List<float>();
    string path = "Assets/timeTrial" + idNum + ".txt";
    void Awake() {
        myInstance = this;
    }

    public class EventClickerClick : UnityEvent { };

    public EventClickerClick eventClickerClick;

    // Use this for initialization
    void Start () {
        starter = false;
        myTime = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (starter) {
            myTime += Time.deltaTime;
        }
        if (numTrial >=10) {
            completeTrial();
        }
    }
    
    public void pressRefine() {
        refineTime = myTime;
        refineArray.Add(refineTime);
        Debug.Log(refineTime);
    }

    public void releaseRefine() {
        selectTime = myTime;
        selectArray.Add(selectTime);    
        Debug.Log(selectTime);
       // Input.GetMouseButtonDown(0);
        eventClickerClick.Invoke();
        myTime = 0;
        starter = false;
        numTrial++;
        //gameObject.GetComponent<timeManager>().enabled = false;
    }

    public void completeTrial() {
        numTrial = 0;
        Debug.Log(refineArray);
        StreamWriter writer = new StreamWriter(path, true);
        foreach (int x in refineArray) {
            writer.WriteLine(refineArray.IndexOf(x));
            writer.WriteLine(selectArray.IndexOf(x));
        }
        writer.Close();

    }
}
