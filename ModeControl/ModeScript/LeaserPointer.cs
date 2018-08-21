using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModeFunc {

public class LeaserPointer : MonoBehaviour, IModeFunc {

	[SerializeField] LineRenderer _leaserPointerRenderer = null;
	private bool _enabled;
	private string _hitObjName;

	public bool enabled {
		get { return _enabled; }
		set { _enabled = value; }
	}

	public string hitObjName { get { return _hitObjName; } }

	void Start () {
		if (_leaserPointerRenderer == null) {
			_leaserPointerRenderer = gameObject.AddComponent<LineRenderer>();
			_leaserPointerRenderer.startWidth = 0.01f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// ヒットしているオブジェクト名を取得
		RaycastHit hitInfo;
		GameObject hitObj;
		MyOVRRayHelper.GetRayHit(out hitInfo, out hitObj);
		_hitObjName = (hitObj != null ? hitObj.name : "");

		if (_enabled == true) {
			// コントローラーから前に伸ばしたRayを作成
			var controller = MyOVRRayHelper.GetController();
			var pointerRay = new Ray(controller.position, controller.transform.forward);
			_leaserPointerRenderer.positionCount = 2;
			_leaserPointerRenderer.SetPosition(0, pointerRay.origin);
			if (hitObj != null) {
				// hitした位置までレーザーを伸ばす
				_leaserPointerRenderer.SetPosition(1, hitInfo.point);
			} else {
				// hitしない場合、Rayの方向に最大長さの位置をもとめてそこまでレーザーを伸ばす
				_leaserPointerRenderer.SetPosition(1, pointerRay.origin + (pointerRay.direction * 100));
			} 
		} else {
			// レーザーをクリア
			_leaserPointerRenderer.positionCount = 0;
		}
	}
}

}
