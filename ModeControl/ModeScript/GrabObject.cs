using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ModeFunc {

public class GrabObject : MonoBehaviour, IModeFunc {

	private GameObject _grabObject = null;
	private Vector3 _grabOffset;
	private float   _grabDistance = 0;
	private Vector3 _gravOldPos;

	public bool enabledThrow;
	public OVRInput.Button grabInputKey;
	public Func<bool> isToFarAction;
	public Func<bool> isToNearAction;
	public float distanceChangeSpeed;

	public GrabObject() {
		this.enabledThrow = true;
		this.grabInputKey = OVRInput.Button.PrimaryIndexTrigger;
		this.isToFarAction  = () => MyOVRInput.GetSwipe(KeyCode.UpArrow);
		this.isToNearAction = () => MyOVRInput.GetSwipe(KeyCode.DownArrow);
		this.distanceChangeSpeed = 0.1f;
	}

	void OnDisable() {
		this.RelaseGrab();
	}

	// update
	void Update () {
		// ヒットしているオブジェクトを取得
		RaycastHit hitInfo;
		GameObject hitObj;
		MyOVRRayHelper.GetRayHit(out hitInfo, out hitObj);
		
		// 掴むのを開始
		if (MyOVRInput.GetDown(this.grabInputKey) && IsGrabable(hitObj)) {
			_grabObject = hitObj;
			_grabDistance = hitInfo.distance;
			_grabOffset = _grabObject.transform.position - hitInfo.point; // ヒットした場所からオブジェクト中心までの距離
			_gravOldPos = _grabObject.transform.position;
		}

		// 掴んだオブジェクトを移動
		if (_grabObject != null) {
			// 距離変更
	        if (this.isToFarAction() ) { _grabDistance += this.distanceChangeSpeed; }
	        if (this.isToNearAction()) { _grabDistance -= this.distanceChangeSpeed; }
			if (_grabDistance < 0.1) { _grabDistance = 0.1f; }

			// 移動
			_gravOldPos = _grabObject.transform.position;
			
			Transform controller = MyOVRRayHelper.GetController();
			Vector3 rayDistancePos = controller.position;
			rayDistancePos += controller.rotation * (Vector3.forward * _grabDistance); // コントローラから_grabDistanceほど進んだ位置
			_grabObject.transform.position = rayDistancePos + _grabOffset;
			
			// 重力を無効にするために速度をゼロに戻しておく
			Rigidbody rigidbody = _grabObject.GetComponent<Rigidbody>();
			if (rigidbody != null) {
				rigidbody.velocity = Vector3.zero;
			}
		}

		// 掴んだオブジェクトを離す
		if (MyOVRInput.GetUp(this.grabInputKey)) {
			this.RelaseGrab();
		}
	}
	
	private bool IsGrabable(GameObject hitObj) {
		if (hitObj == null) { return false; }
		if (hitObj.GetComponent        <GrabUnableObject>() != null) { return false; }
		if (hitObj.GetComponentInParent<GrabUnableObject>() != null) { return false; }
		return true;
	}
	
	private void RelaseGrab() {
		if (_grabObject == null) { return; }
		
		Rigidbody rigidbody = _grabObject.GetComponent<Rigidbody>();
		if (rigidbody != null && this.enabledThrow) {
			Vector3 force = _grabObject.transform.position - _gravOldPos;
			rigidbody.velocity = force * 30f;
		}
		_grabObject = null;
	}
}

}
