using UnityEngine;
using System;
using System.Collections.Generic;

public class PupilMarker
{
	public string name;
	private Color _color = Color.white;
	public Color color
	{
		get { return _color; }
		set
		{
			_color = value;

			if (material != null)
				material.color = _color;
		}
	}
	public Vector3 position;
	private Material material;
	private GameObject _gameObject;
	private GameObject gameObject
	{
		get
		{
			if (_gameObject == null)
			{
				_gameObject = GameObject.Instantiate (Resources.Load<GameObject> ("MarkerObject"));
				_gameObject.name = this.name;
				material = new Material (Resources.Load<Material> ("Materials/MarkerMaterial"));
				_gameObject.GetComponent<MeshRenderer> ().material = material;
				_gameObject.transform.parent = this.camera.transform;
				material.color = this.color;
			}
			return _gameObject;
		}
	}
				
	private Camera _camera;
	public Camera camera
	{
		get
		{
			if (_camera == null)
			{
				_camera = Camera.main;
			}
			return _camera;
		}
		set
		{
			_camera = value;
			gameObject.transform.parent = _camera.transform;
		}
	}

	public PupilMarker(string name, Color color,bool hide=false)
	{
		this.name = name;
		this.color = color;
        if(hide)this.SetScale(0);
		this.camera = PupilSettings.Instance.currentCamera;
	}

    public class Tuple
    {
        public DateTime Item1;
        public Vector3 Item2;
        public Quaternion Item3;
        public Tuple(DateTime a,Vector3 b,Quaternion c) {
            Item1 = a;
            Item2 = b;
            Item3 = c;
        }
    }

    public const double Latency = 2000;//(99 + 120) / 2;
    public static Queue<Tuple> CameraHistory
        = new Queue<Tuple>();
    Vector3 Copy(Vector3 a) {
        return new Vector3(a.x, a.y, a.z);
    }
    Quaternion Copy(Quaternion a) {
        return new Quaternion(a.x, a.y, a.z, a.w);
    }
    public void UpdateCalibratePosition(Vector2 newPosition) {
        position.x = newPosition.x;
        position.y = newPosition.y;
        position.z = PupilTools.CalibrationType.vectorDepthRadius[0].x;
        var time = DateTime.Now;
        var cameraData = new Tuple(time, Copy(camera.transform.position), Copy(camera.transform.rotation));
        CameraHistory.Enqueue(cameraData);
        while (CameraHistory.Peek().Item1 < DateTime.Now.AddMilliseconds(-Latency)) CameraHistory.Dequeue();

        camera.transform.position = Copy(CameraHistory.Peek().Item2);
        camera.transform.rotation = Copy(CameraHistory.Peek().Item3);
        gameObject.transform.position = camera.ViewportToWorldPoint(position);
        camera.transform.position = Copy(cameraData.Item2);
        //UnityEngine.Debug.Log("time=" + time.ToString() + "\t count=" + CameraHistory.Count.ToString() + "\t queue.front = {" + CameraHistory.Peek().Item2.ToString() + ", " + CameraHistory.Peek().Item3.ToString()+"}");        camera.transform.rotation = Copy(cameraData.Item3);
        UpdateOrientation();
    }
    Vector3 RotationDifCamera(Vector3 a,Vector3 b) {
        a -= camera.transform.position;
        b -= camera.transform.position;
        return b.normalized - a.normalized;
    }
    public void UpdatePosition(Vector2 newPosition)
	{		
		position.x = newPosition.x;
		position.y = newPosition.y;
		position.z = PupilTools.CalibrationType.vectorDepthRadius[0].x;
        if(UserStudyOneScript.isRefining) {
            var cameraRotationDifference = camera.transform.rotation.eulerAngles.normalized - UserStudyOneScript.storedCameraRotation.eulerAngles.normalized;
            var wp = camera.ViewportToWorldPoint(position);
            UnityEngine.Debug.Log(wp.ToString());
            float t = 1e5f, scale = 0.9f;
            while(t>1e-5) {
                var curDif = RotationDifCamera(camera.ViewportToWorldPoint(position), wp);
                for (int _ = 0; _ < 2; _++) {
                    position.x += t;
                    if (RotationDifCamera(camera.ViewportToWorldPoint(position), wp).magnitude > curDif.magnitude) position.x -= t;
                    position.y += t;
                    if (RotationDifCamera(camera.ViewportToWorldPoint(position), wp).magnitude > curDif.magnitude) position.y -= t;
                    t *= -1;
                }
                t *= scale;
            }
            gameObject.transform.position = camera.ViewportToWorldPoint(position);
        }
        else {
            gameObject.transform.position = camera.ViewportToWorldPoint(position);
        }
		UpdateOrientation ();
	}
	public void UpdatePosition(Vector3 newPosition)
	{
		position = newPosition;
		gameObject.transform.localPosition = position;
		UpdateOrientation ();
	}
    public void UpdatePositionDisconnected(float x, float y, float z) {
        position.x = x;
        position.y = y;
        position.z = z;
        gameObject.transform.position = camera.ViewportToWorldPoint(position);
    }
    public Vector3 WorldPoint() { return camera.ViewportToWorldPoint(position); }
	public void UpdatePosition(float[] newPosition)
	{
		if (PupilTools.CalibrationMode == Calibration.Mode._2D)
		{
			if (newPosition.Length == 2)
			{
				position.x = newPosition[0];
				position.y = newPosition[1];
				position.z = PupilTools.CalibrationType.vectorDepthRadius[0].x;
				gameObject.transform.position = camera.ViewportToWorldPoint(position);
			} 
			else
			{
				Debug.Log ("Length of new position array does not match 2D mode");
			}
		}
		else if (PupilTools.CalibrationMode == Calibration.Mode._3D)
		{
			if (newPosition.Length == 3)
			{
				position.x = newPosition[0];
				position.y = newPosition[1];
				position.z = newPosition[2];
				gameObject.transform.localPosition = position;
			} 
			else
			{
				Debug.Log ("Length of new position array does not match 3D mode");
			}
		}
		UpdateOrientation ();
	}
	private void UpdateOrientation()
	{
		gameObject.transform.LookAt (this.camera.transform.position);
	}

//	public void Initialize(bool isActive)
//	{
//		gameObject = GameObject.Instantiate (Resources.Load<GameObject> ("MarkerObject"));
//		gameObject.name = this.name;
//		gameObject.GetComponent<MeshRenderer> ().material = new Material (Resources.Load<Material> ("MarkerMaterial"));
//		gameObject.GetComponent<MeshRenderer> ().material.SetColor ("_EmissionColor", this.color);
//		gameObject.SetActive (isActive);
//		gameObject.transform.parent = this.camera.transform;
//		//				gameObject.hideFlags = HideFlags.HideInHierarchy;
//	}

	public static bool TryToSetActive(PupilMarker marker, bool toggle)
	{
		if (marker != null)
		{
			if (marker.gameObject != null)
				marker.gameObject.SetActive (toggle);
			return true;
		}
		return false;
	}

	public void SetScale (float value)
	{
		if (gameObject.transform.localScale.x != value)
			gameObject.transform.localScale = Vector3.one * value;
	}

	public static bool TryToReset (PupilMarker marker)
	{
		if (marker != null)
		{
			marker.camera = PupilSettings.Instance.currentCamera;
			marker.gameObject.SetActive (true);
			return true;
		}
		return false;
	}
}