using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModeFunc {

public class MoveWarp : MonoBehaviour, IModeFunc {

	Transform _moveTarget;
	[SerializeField] bool _moveCameraRigParent = true;
	public OVRInput.Button warpInputKey;
	public float baseHeight = 0.0f;

	// コンストラクタ
	public MoveWarp() {
		this.warpInputKey = OVRInput.Button.PrimaryIndexTrigger;
	}

	void Start () {
		_moveTarget = GameObject.Find("OVRCameraRig").transform;
		if (_moveCameraRigParent) { _moveTarget = _moveTarget.parent; }
		if (this.baseHeight == 0.0f) {
			this.baseHeight = _moveTarget.position.y;
		}
	}
	
	void Update () {
		if (MyOVRInput.GetDown(this.warpInputKey)) {
			// ヒットしているオブジェクト名を取得
			RaycastHit hitInfo;
			GameObject hitObj;
			MyOVRRayHelper.GetRayHit(out hitInfo, out hitObj);
			
			if (hitObj != null) {
				Vector3 newPos = hitInfo.point;
				newPos += new Vector3(0, this.baseHeight, 0);
				_moveTarget.position = newPos;
			}
		}
	}
}

}
