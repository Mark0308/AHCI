using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HoloToolkit.Unity.InputModule;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UserStudyOneScript : MonoBehaviour//, IInputClickHandlerS
{
	public Vector2 gazePointCenter;
    public string dataURL;
	public Material shaderMaterial;
    public Button Center;
    public GameObject buttonCanvas;
    public Camera demo;
    public string gazeData = "";
    public int idNum;

    public static int currentOrder = 0;
    public int conditionID = 0;
    public HoloToolkit.Unity.InputModule.Cursor cursor;
    private string path;
    bool isTraining = false;
    bool isStudy = false;
    int[] order = new int[32];
    void Start () 
	{
        Debug.Log("in view");

    }

    void setCenter() {
        RaycastHit hit;

        Ray ray = Camera.main.ViewportPointToRay(gazePointCenter);
        if (Physics.Raycast(ray, out hit)) {
            Debug.Log("Hit " + hit.transform.name);
            if (hit.transform.gameObject.name.Equals("Center")) {
                // dwellTime += Time.deltaTime;
                //  if (dwellTime >= 1) {
                Center.onClick.Invoke();
                //     dwellTime = 0;
                // }
            }
        }
        Debug.Log(gazePointCenter);
    }

	void OnEnable()
	{
		if (PupilTools.IsConnected)
		{
			PupilTools.IsGazing = true;
			PupilTools.SubscribeTo ("gaze");
		}
	}

	public bool monoColorMode = true;
    bool Lock = false;
    public PupilMarker headCursor;
    public PupilMarker lockCursor;
    public Vector3 lockEyeWorldPoint;
    public Vector3 lockHeadForward;
    public static bool isRefining = false;
    public Vector2 scaler = new Vector2(-0.5f, 0.5f);
    float dwellTime = 0;
    targetClicker targetclicker = new targetClicker();

    public static Quaternion storedCameraRotation;

    

    void Update()
	{
        
        if (PupilTools.IsConnected && PupilTools.IsGazing && (isTraining || isStudy)) {
            //timeManager time = new timeManager();
            if (isStudy && !Lock) {
                //set target to next index
                //if no target set to false and save data
                if (currentOrder < 24) {
                    Debug.Log("set random target for study");
                    targetclicker.setTargetActive(order[currentOrder]);
                    currentOrder++;
                    Lock = true;
                }
                else {
                    isStudy = false;
                    streamWriter.Close();
                    currentOrder = 0;
                    return;
                }

                //save data
            }
            else if (isTraining && !Lock) {
                Debug.Log("set random target for training");
                //random generate data
                int trainingIndex = UnityEngine.Random.Range(0, order.Length);
                targetclicker.setTargetActive(trainingIndex);
                Lock = true;
            }
                
            if (isRefining) {
                Vector2 deltaHead = this.transform.forward - lockHeadForward;
                gazePointCenter = lockCursor.camera.WorldToViewportPoint(lockEyeWorldPoint);
                lockCursor.UpdatePosition(gazePointCenter);
                headCursor.UpdatePosition(gazePointCenter + Vector2.Scale(scaler, deltaHead));
            } else {
                gazePointCenter = PupilData._2D.GazePosition;
            }

            
            //missing head point
            streamWriter.WriteLine(gazePointCenter);//save in json
            if (Input.GetKeyDown(KeyCode.R)) {
                if (!isRefining) {
                    Debug.Log("Enter refinement mode.");
                    isRefining = true;
                    lockHeadForward = this.transform.forward;
                    PupilGazeTracker.Instance.hideGaze();
                    headCursor = new PupilMarker("Head", Color.blue);
                    headCursor.UpdatePosition(gazePointCenter);
                    lockCursor = new PupilMarker("Lock", Color.red);
                    lockCursor.UpdatePosition(gazePointCenter);
                    lockEyeWorldPoint = lockCursor.WorldPoint();
                    storedCameraRotation = Camera.main.transform.rotation;
                    timeManager.myInstance.pressRefine();
                    streamWriter.WriteLine("enter refine:" + gazePointCenter);
                    Debug.Log(gazePointCenter);
                }
            }
            else if (Input.GetKeyUp(KeyCode.R) && isRefining) {
                //Debug.Log(time.selectTime);
                Debug.Log("Exit refinement mode.");
                //write to Txt file
                streamWriter.WriteLine("exit refine:" + gazePointCenter);
                Debug.Log(gazePointCenter);
                
                // AssetDatabase.ImportAsset(path);
                // TextAsset asset = Resources.Load("test");
                isRefining = false;
                PupilGazeTracker.Instance.showGaze();
                PupilMarker.TryToSetActive(headCursor, false);
                PupilMarker.TryToSetActive(lockCursor, false);
                timeManager.myInstance.releaseRefine();
                Lock = false;
                //send request to upload movement data
            }
        }
        //change condition
        if (Input.GetKey(KeyCode.Z) && !isStudy) {
            Debug.Log("switch to condition1");
            conditionID = 0;
            path = "Assets/test" + idNum.ToString() + "_" + conditionID.ToString() + ".txt"; //txt save file
            fileStream = new FileStream(path, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
            isTraining = true;
            setCenter();
        }
        else if (Input.GetKey(KeyCode.X) && !isStudy) {
            Debug.Log("switch to condition2");
            conditionID = 1;
            path = "Assets/test" + idNum.ToString() + "_" + conditionID.ToString() + ".txt"; //txt save file
            fileStream = new FileStream(path, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
            isTraining = true;
            setCenter();
        }
        else if (Input.GetKey(KeyCode.C) && !isStudy) {
            Debug.Log("switch to condition3");
            conditionID = 2;
            path = "Assets/test" + idNum.ToString() + "_" + conditionID.ToString() + ".txt"; //txt save file
            fileStream = new FileStream(path, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
            isTraining = true;
            setCenter();
        }
        else if (Input.GetKey(KeyCode.V) && !isStudy) {
            Debug.Log("switch to condition4");
            conditionID = 3;
            path = "Assets/test" + idNum.ToString() + "_" + conditionID.ToString() + ".txt"; //txt save file
            fileStream = new FileStream(path, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
            isTraining = true;
            setCenter();
        }
        else if (Input.GetKey(KeyCode.Space)) {
            if (isTraining) {
                isTraining = false;
                generateOrder();  
                isStudy = true;
            }
        }
    }

    public void generateOrder() {
        System.Random rnd = new System.Random();
        for (int _ = 0; _ < 24; _++)
            order[_] = rnd.Next(0, 24);
    }

    /*
    void OnInputClicked(InputClickedEventData eventData) {
        Debug.Log("clicking");
    }
    */

    public void oneToPointFive() {
        scaler = new Vector2(2 / 3, 2 / 3);
        buttonCanvas.SetActive(true);
    }
    public void oneToTwo() {

        scaler = new Vector2(1 / 2, 1 / 2);
        buttonCanvas.SetActive(true);
    }
    public void oneToThree() {

        scaler = new Vector2(1 / 3, 1 / 3);
        buttonCanvas.SetActive(true);
    }
    public void propotional() {

    }

    static FileStream fileStream=null;
    static StreamWriter streamWriter = null;
    static UserStudyOneScript() {
        string path = null;
    #if UNITY_EDITOR
            path = "Assets/gazeInfo.json";
    #endif
        fileStream = new FileStream(path, FileMode.Create);
        streamWriter = new StreamWriter(fileStream);
    }
    static System.DateTime lastRefreshTime = System.DateTime.Now;

    //save corridinate for each frame to json 
    void SaveGazeInfo(Vector2 gaze) {
        //gazeData += gaze;
    #if UNITY_EDITOR
        streamWriter.Write(gaze);
        if ((System.DateTime.Now - lastRefreshTime).TotalMilliseconds > 500) {
            UnityEditor.AssetDatabase.Refresh();
            lastRefreshTime = System.DateTime.Now;
        }
    #endif
    }
    void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		//if (monoColorMode)
		//{
			//shaderMaterial.SetFloat ("_highlightThreshold", 0.1f);
            //shaderMaterial.SetVector("_viewportGazePosition", gazePointCenter);
			//Graphics.Blit (source, destination, shaderMaterial);
		//} else 
			Graphics.Blit (source, destination);
	}

    IEnumerator sendData(string data, string index) {
        using (UnityWebRequest request = UnityWebRequest.Post(dataURL + "&name=" + index, data)) {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
                Debug.Log("error on sending the data : " + request.error);
            else
                Debug.Log("sent data");   
        }
    }
}
