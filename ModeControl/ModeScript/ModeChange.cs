using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ModeFunc {

public class ModeChange : MonoBehaviour, IModeFunc {

	private ModeSelector _modeSelector;
	private Transform _rightHandAnchor;

	public Func<bool> isPrevAction;
	public Func<bool> isNextAction;

	// コンストラクタ
	public ModeChange() {
		// 前/次モードに変更するアクションの既定値は、回転加速度が一定を超えた判定
		this.isPrevAction = () => this.isAccOver(vec => vec.x < -20f) || Input.GetKeyDown(KeyCode.F1);
		this.isNextAction = () => this.isAccOver(vec => vec.x > +20f) || Input.GetKeyDown(KeyCode.F2);
	}

	// 加速度が一定を超えた判定
	private int ignoreAccOverFrame = 0;
	private bool isAccOver(Func<Vector3, bool> isOver) {
		if (Time.frameCount < ignoreAccOverFrame) { return false; }

		var controller = OVRInput.GetActiveController();
		Vector3 accVec = OVRInput.GetLocalControllerAcceleration(controller);
		Vector3 localAccVec = _rightHandAnchor.InverseTransformVector(accVec);

		if (isOver(localAccVec)) {
			ignoreAccOverFrame = Time.frameCount + 30; // 以降しばらくは無視
			return true;
		} else {
			return false;
		}
	}

	// モード機能変更するためのオブジェクトを保持
	public void SetModeSelector (ModeSelector modeSelector) {
		_modeSelector = modeSelector;
	}

	// start
	void Start() {
		_rightHandAnchor = GameObject.Find("RightHandAnchor").transform;
	}

	// update	
	void Update () {
		if (this.isPrevAction()) { _modeSelector.SelectPrev(); }
		if (this.isNextAction()) { _modeSelector.SelectNext(); }
	}

}

}
