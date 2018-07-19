using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyOVRInput : MonoBehaviour {
	private GameObject _ovrCameraRig;

	// 矢印キーで視点移動エミュレート
	// ------------------------------------------------------------------------------------------
	void Start () {
		_ovrCameraRig = GameObject.Find("OVRCameraRig");
	}
	
	void Update () {
		#if UNITY_EDITOR
			if (_ovrCameraRig == null) { return; }
		    if (Input.GetKey(KeyCode.RightArrow))
		    {
		      _ovrCameraRig.transform.Rotate(0, 1, 0, Space.World);
		    }
		    else if (Input.GetKey(KeyCode.LeftArrow))
		    {
		      _ovrCameraRig.transform.Rotate(0, -1, 0, Space.World);
		    }
		    else if (Input.GetKey(KeyCode.UpArrow))
		    {
		      _ovrCameraRig.transform.Rotate(-1, 0, 0);
		    }
		    else if (Input.GetKey(KeyCode.DownArrow))
		    {
		      _ovrCameraRig.transform.Rotate(1, 0, 0);
		    }
		#endif
	}

	// キーボードでボタン、タッチをエミュレートするOVRInput.GetXXX
	// ------------------------------------------------------------------------------------------
	private static Dictionary<OVRInput.Button, KeyCode> _buttonKeycodeMap = null;
	private static Dictionary<OVRInput.Touch , KeyCode> _touchKeycodeMap = null;
	private static KeyCode _reCenterKeyMap;
	// Init
	public static void InitOVRInput() {
		if (_buttonKeycodeMap != null) { return; }
		
		_buttonKeycodeMap = new Dictionary<OVRInput.Button, KeyCode>();
		_buttonKeycodeMap.Add(OVRInput.Button.PrimaryIndexTrigger, KeyCode.Space);
		_buttonKeycodeMap.Add(OVRInput.Button.PrimaryTouchpad    , KeyCode.Return);
		_buttonKeycodeMap.Add(OVRInput.Button.One                , KeyCode.Return);
		_buttonKeycodeMap.Add(OVRInput.Button.Back               , KeyCode.Escape); // OculusGoコントローラーの戻るボタンは、OVRInput.Button.Backでは取れない。あれは「Androidの戻るボタン」なのでKeyCode.Escapeで取れる
		_buttonKeycodeMap.Add(OVRInput.Button.Two                , KeyCode.Escape);
		_buttonKeycodeMap.Add(OVRInput.Button.Up                 , KeyCode.T);
		_buttonKeycodeMap.Add(OVRInput.Button.Down               , KeyCode.G);
		_buttonKeycodeMap.Add(OVRInput.Button.Right              , KeyCode.H);
		_buttonKeycodeMap.Add(OVRInput.Button.Left               , KeyCode.F);

		_touchKeycodeMap = new Dictionary<OVRInput.Touch, KeyCode>();
		_touchKeycodeMap.Add(OVRInput.Touch.PrimaryTouchpad, KeyCode.LeftShift);
		_touchKeycodeMap.Add(OVRInput.Touch.One            , KeyCode.LeftShift);

		_reCenterKeyMap = KeyCode.Tab;
	}
	// Get - Button
	public static bool Get(OVRInput.Button input) {
		InitOVRInput();
		if (_buttonKeycodeMap.ContainsKey(input)) {
			if (Input.GetKey(_buttonKeycodeMap[input])) { return true; }
		}
		return OVRInput.Get(input);
	}
	// Get - Touch
	public static bool Get(OVRInput.Touch input) {
		InitOVRInput();
		if (_touchKeycodeMap.ContainsKey(input)) {
			if (Input.GetKey(_touchKeycodeMap[input])) { return true; }
		}
		return OVRInput.Get(input);
	}
	// Get - Axis
	public static Vector2 Get(OVRInput.Axis2D input) {
		if (Input.GetKey(KeyCode.W)) { return new Vector2( 0, +1); }
		if (Input.GetKey(KeyCode.S)) { return new Vector2( 0, -1); }
		if (Input.GetKey(KeyCode.D)) { return new Vector2(+1,  0); }
		if (Input.GetKey(KeyCode.A)) { return new Vector2(-1,  0); }
		return OVRInput.Get(input);
	}
	// GetDown - Button
	public static bool GetDown(OVRInput.Button input) {
		InitOVRInput();
		if (_buttonKeycodeMap.ContainsKey(input)) {
			if (Input.GetKeyDown(_buttonKeycodeMap[input])) { return true; }
		}
		return OVRInput.GetDown(input);
	}
	// GetDown - Touch
	public static bool GetDown(OVRInput.Touch input) {
		InitOVRInput();
		if (_touchKeycodeMap.ContainsKey(input)) {
			if (Input.GetKeyDown(_touchKeycodeMap[input])) { return true; }
		}
		return OVRInput.GetDown(input);
	}
	// GetUp - Button
	public static bool GetUp(OVRInput.Button input) {
		InitOVRInput();
		if (_buttonKeycodeMap.ContainsKey(input)) {
			if (Input.GetKeyUp(_buttonKeycodeMap[input])) { return true; }
		}
		return OVRInput.GetUp(input);
	}
	// GetUp - Touch
	public static bool GetUp(OVRInput.Touch input) {
		InitOVRInput();
		if (_touchKeycodeMap.ContainsKey(input)) {
			if (Input.GetKeyUp(_touchKeycodeMap[input])) { return true; }
		}
		return OVRInput.GetUp(input);
	}
	// Recenter
	public static bool GetControllerWasRecentered() {
		return OVRInput.GetControllerWasRecentered(OVRInput.GetActiveController()) ||
			   Input.GetKeyDown(_reCenterKeyMap);
	}
	// Axisを方向に変換
	public static KeyCode Axis2Arrow(Vector2 axis) {
		if (axis.x > 0.7 && (-0.5 < axis.y && axis.y< 0.5)) { return KeyCode.RightArrow; }
		if (axis.x< -0.7 && (-0.5 < axis.y && axis.y< 0.5)) { return KeyCode.LeftArrow;  }
		if (axis.y > 0.6 && (-0.5 < axis.x && axis.x< 0.5)) { return KeyCode.UpArrow;    } // 上は反応しづらい
		if (axis.y< -0.7 && (-0.5 < axis.x && axis.x< 0.5)) { return KeyCode.DownArrow;  }
		return KeyCode.None;
	}
}
