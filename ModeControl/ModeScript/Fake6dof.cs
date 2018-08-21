using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ModeFunc {

public class Fake6dof : MonoBehaviour, IModeFunc {

	[SerializeField] Transform _camera;
	public float _cameraHight;

	void Start () {
		_camera = GameObject.Find("OVRCameraRig").transform;
		if (_cameraHight == 0) {
			_cameraHight = _camera.localPosition.y;
		}
	}
	
	void Update () {
		// コントローラーを押している間は、カメラ位置を調整
		if (MyOVRUtil.Get(OVRInput.Button.PrimaryIndexTrigger)) {
			// コントローラーから前に伸ばしたRayを作成して、オフセットを得る
			Transform controller = MyOVRUtil.GetController();
			var pointerRay = new Ray(controller.position, controller.transform.forward);

			Vector3 rayPointFm = _camera.transform.InverseTransformPoint(pointerRay.GetPoint(0));
			Vector3 rayPointTo = _camera.transform.InverseTransformPoint(pointerRay.GetPoint(_cameraHight));
			
			Vector3 cameraOffset = rayPointTo - rayPointFm;
	
			// カメラのローカル座標を設定
			_camera.localPosition = cameraOffset;
        }
	}

}

}
