using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModeFunc {

public class MoveSegwayLike : MonoBehaviour, IModeFunc {

	Transform _moveTarget;
	Vector3 _basePos;
	float SQRT2 = Mathf.Sqrt(2f);
	[SerializeField] bool _moveCameraRigParent = true;
	public float _maxSpeed = 4.0f;

	void Start () {
		_moveTarget = GameObject.Find("OVRCameraRig").transform;
		if (_moveCameraRigParent) { _moveTarget = _moveTarget.parent; }
	}
	
	void Update () {
		if (MyOVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) {
			Transform controller = MyOVRRayHelper.GetController();
			_basePos = controller.rotation * Vector3.forward;
		}
		if (MyOVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
			Transform controller = MyOVRRayHelper.GetController();
			Vector3 currPos = controller.rotation * Vector3.forward;
			Vector3 diffVec = currPos - _basePos;
			
			float normalizedMagnitute = Mathf.InverseLerp(0f, SQRT2, diffVec.magnitude);
			
			if (normalizedMagnitute > 0.05f) {
				_moveTarget.position += NormalizedLandscape(diffVec) * (normalizedMagnitute * _maxSpeed * Time.deltaTime);
			}
		}
	}

	// 水平移動以外の成分を取り除く
	private Vector3 NormalizedLandscape(Vector3 vec) {
		return new Vector3(vec.x, 0, vec.z).normalized;
	}

}

}
