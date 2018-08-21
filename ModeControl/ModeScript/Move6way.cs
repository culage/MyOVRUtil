using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModeFunc {

public class Move6way : MonoBehaviour, IModeFunc {

	Transform _moveTarget;
	[SerializeField] bool _moveCameraRigParent = true;
	public float _maxSpeed = 3.0f;

	void Start () {
		_moveTarget = GameObject.Find("OVRCameraRig").transform;
		if (_moveCameraRigParent) { _moveTarget = _moveTarget.parent; }
	}
	
	void Update () {
		if (MyOVRInput.Get(OVRInput.Touch.PrimaryTouchpad)) {
			Vector2 axis = MyOVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
			if (MyOVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
				// トリガーを押しているときは上下
				_moveTarget.position += Vector3.up * (axis.y * _maxSpeed * Time.deltaTime);
			} else {
				// それ以外は前後左右
				Transform camera = Camera.main.transform;
				_moveTarget.position += NormalizedLandscape(camera.rotation * Vector3.right)   * (axis.x * _maxSpeed * Time.deltaTime);
				_moveTarget.position += NormalizedLandscape(camera.rotation * Vector3.forward) * (axis.y * _maxSpeed * Time.deltaTime);
			}
		}
	}

	// 水平移動以外の成分を取り除く
	private Vector3 NormalizedLandscape(Vector3 vec) {
		return new Vector3(vec.x, 0, vec.z).normalized;
	}
}

}
