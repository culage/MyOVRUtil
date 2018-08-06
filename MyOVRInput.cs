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
	private static int _lastEscapeFrame = 0;
	public static bool GetDown(OVRInput.Button input) {
		InitOVRInput();
		if (_buttonKeycodeMap.ContainsKey(input)) {
			KeyCode keyCode = _buttonKeycodeMap[input];
			if (keyCode == KeyCode.Escape) {
				// Escape(Backボタン)は何故か連続で押された判定になることがあるので、押した後一定時間押せなくする
				if (Time.frameCount > _lastEscapeFrame + 5 && Input.GetKeyDown(KeyCode.Escape)) {
					_lastEscapeFrame = Time.frameCount;
					return true;
				}
			} else {
				if (Input.GetKeyDown(keyCode)) { return true; }
			}
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
	// スワイプを取得
	static class OldInputValue {
		private static int _savedFrameCount;
		private static bool _nowIsTouch, _oldIsTouch;
		private static Vector2 _nowAxis, _oldAxis;
		private static Vector2 _touchStartAxis;
		private static List<Dictionary<KeyCode, bool>> _swipeHistory = new List<Dictionary<KeyCode, bool>>();

		public static bool    isTouch        { get{ ShiftOldValue(); return _oldIsTouch;     } }
		public static Vector2 axis           { get{ ShiftOldValue(); return _oldAxis;        } }
		public static Vector2 touchStartAxis { get{ ShiftOldValue(); return _touchStartAxis; } }
		public static List<Dictionary<KeyCode, bool>> swipeHistory {
			get {
				ShiftOldValue();
				return _swipeHistory;
			}
		}

		public static void SaveOld(bool isTouch, Vector2 axis, Dictionary<KeyCode, bool> swipeResult) {
			_savedFrameCount = Time.frameCount;
			_nowIsTouch = isTouch;
			_nowAxis    = axis;

			_swipeHistory.Add( new Dictionary<KeyCode, bool>(swipeResult) );
			if (_swipeHistory.Count > 60) { _swipeHistory.RemoveAt(0); }

			if (_oldIsTouch == false && isTouch == true) {
				_touchStartAxis = axis;
			}
		}
		public static bool IsNewFrame() {
			return _savedFrameCount != Time.frameCount;
		}
		private static void ShiftOldValue() {
			if (IsNewFrame()) {
				_oldIsTouch     = _nowIsTouch;
				_oldAxis        = _nowAxis;
			}
		}
		public static void FillSwipeHistory(KeyCode swipeArrow, bool fillValue) {
			foreach(var swipe in _swipeHistory) {
				swipe[swipeArrow] = fillValue;
			}
		}
	}
	private static Dictionary<KeyCode, bool> _swipeResult = new Dictionary<KeyCode, bool>();
	public static bool GetSwipe(KeyCode swipeArrow) {
		if (OldInputValue.IsNewFrame()) {
			_swipeResult[KeyCode.RightArrow] = false;
			_swipeResult[KeyCode.LeftArrow ] = false;
			_swipeResult[KeyCode.UpArrow   ] = false;
			_swipeResult[KeyCode.DownArrow ] = false;

			if (Input.GetKey(KeyCode.H)) { _swipeResult[KeyCode.RightArrow] = true; }
			if (Input.GetKey(KeyCode.F)) { _swipeResult[KeyCode.LeftArrow ] = true; }
			if (Input.GetKey(KeyCode.T)) { _swipeResult[KeyCode.UpArrow   ] = true; }
			if (Input.GetKey(KeyCode.G)) { _swipeResult[KeyCode.DownArrow ] = true; }

			bool isTouch = OVRInput.Get(OVRInput.Touch.PrimaryTouchpad);
			Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

			if (OldInputValue.isTouch && isTouch &&
				(OldInputValue.touchStartAxis - axis).magnitude >= 0.1) {
				if (OldInputValue.axis.x < axis.x) { _swipeResult[KeyCode.RightArrow] = true; }
				if (OldInputValue.axis.x > axis.x) { _swipeResult[KeyCode.LeftArrow ] = true; }
				if (OldInputValue.axis.y < axis.y) { _swipeResult[KeyCode.UpArrow   ] = true; }
				if (OldInputValue.axis.y > axis.y) { _swipeResult[KeyCode.DownArrow ] = true; }
			}

			OldInputValue.SaveOld(isTouch, axis, _swipeResult);
		}

		return _swipeResult[swipeArrow];
	}
	// スワイプ開始を取得
	public static bool GetSwipeDown(KeyCode swipeArrow) {
		int prev1OnCount = 0;
		int prev2OffCount = 0;

		int i=OldInputValue.swipeHistory.Count - 1;
		for(; i >= 0; i--) {
			var swipe = OldInputValue.swipeHistory[i];
			if (swipe[swipeArrow] == true) { prev1OnCount++; }
			else { break; }
		}
		for(; i >= 0; i--) {
			var swipe = OldInputValue.swipeHistory[i];
			if (swipe[swipeArrow] == false) { prev2OffCount++; }
			else { break; }
		}

		// 10フレーム以上スワイプしてない→10フレーム以上スワイプしてる……のとき
		bool result = (prev2OffCount >= 10 && prev1OnCount >= 10);
		if (result) { OldInputValue.FillSwipeHistory(swipeArrow, true); }
		return result;
	}
	// スワイプ終了を取得
	public static bool GetSwipeUp(KeyCode swipeArrow) {
		int prev1OffCount = 0;
		int prev2OnCount = 0;

		int i=OldInputValue.swipeHistory.Count - 1;
		for(; i >= 0; i--) {
			var swipe = OldInputValue.swipeHistory[i];
			if (swipe[swipeArrow] == false) { prev1OffCount++; }
			else { break; }
		}
		for(; i >= 0; i--) {
			var swipe = OldInputValue.swipeHistory[i];
			if (swipe[swipeArrow] == true) { prev2OnCount++; }
			else { break; }
		}

		// 10フレーム以上スワイプしてる→10フレーム以上スワイプしてない……のとき
		bool result = (prev2OnCount >= 10 && prev1OffCount >= 10);
		if (result) { OldInputValue.FillSwipeHistory(swipeArrow, false); }
		return result;
	}
}
