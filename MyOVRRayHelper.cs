using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyOVRRayHelper : MonoBehaviour {
	// イベントを左右コントローラー切り替えに対応
	// ------------------------------------------------------------------------------------------
	void Update() {
		if (Time.frameCount % 60 != 0) { return; }
		
		var rayTransform = MyOVRRayHelper.GetController();

		var eventSystem = GameObject.Find("EventSystem");
		if (eventSystem != null) {
			eventSystem.GetComponent<UnityEngine.EventSystems.OVRInputModule>().rayTransform = rayTransform;
	  	}

		var pointer = GameObject.Find("OVRGazePointer");
		if (pointer != null) {
			pointer.GetComponent<OVRGazePointer>().rayTransform = rayTransform;
		}
	}
	
	// コントローラー取得
	// ------------------------------------------------------------------------------------------
	private static Transform _rightController = null;
	private static Transform _leftController  = null;
	private static Transform _centerEyeAnchor = null;
	public static Transform GetController() {
		// init
		if (_rightController == null) {
			GameObject rightAnchor = GameObject.Find("RightHandAnchor");
			_rightController = rightAnchor.transform.Find("TrackedRemote");

			GameObject leftAnchor = GameObject.Find("LeftHandAnchor");
			_leftController = leftAnchor.transform.Find("TrackedRemote");

			_centerEyeAnchor = GameObject.Find("OVRCameraRig").transform;
		}

		// get controller
		var controller = OVRInput.GetActiveController();
		if (controller == OVRInput.Controller.RTrackedRemote) { return _rightController; }
		if (controller == OVRInput.Controller.LTrackedRemote) { return _leftController; }
		return _centerEyeAnchor;
	}

	// コントローラーから伸ばしたRayがヒットした位置の情報取得
	// ------------------------------------------------------------------------------------------
	private static int _currentFrameCount;
	private static RaycastHit _currentHitInfo;
	public static void GetRayHit(out RaycastHit outHitInfo, out GameObject outHitObj) {
		// 同じ時間であればキャッシュを使う
		if (Time.frameCount == _currentFrameCount) {
			outHitInfo = _currentHitInfo;
			outHitObj  = _currentHitInfo.collider.gameObject;
		}

		// コントローラーから前に伸ばしたRayを作成
		var controller = MyOVRRayHelper.GetController();
		var pointerRay = new Ray(controller.position, controller.transform.forward);

		// Rayがヒットした位置を取得
		if (Physics.Raycast(pointerRay, out _currentHitInfo, 100f)) {
			outHitInfo = _currentHitInfo;
			outHitObj  = _currentHitInfo.collider.gameObject;
		} else {
			outHitInfo = _currentHitInfo;
			outHitObj  = null;
		}
	}
}
