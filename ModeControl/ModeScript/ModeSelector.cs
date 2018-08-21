using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModeFunc {

public interface IModeFunc {
	bool enabled { set; }
}

public class ModeSelector {

	public event System.Action<string> onSelect;
	private void OnSelect(string mode) { if (onSelect != null) { onSelect(mode); }	}

	private string _currentMode;
	private Dictionary<string, List<IModeFunc>> _modeToActiveFuncList;
	private List<IModeFunc> _allModeFuncList;

	// コンストラクタ
	public ModeSelector() {
		_currentMode = "";
		_modeToActiveFuncList = new Dictionary<string, List<IModeFunc>>();
		_allModeFuncList = new List<IModeFunc>();
	}

	// モード追加
	public void AddMode(string mode, IModeFunc[] modeFuncArray) {
		_modeToActiveFuncList.Add(mode, new List<IModeFunc>(modeFuncArray));
		foreach(IModeFunc modeFunc in modeFuncArray) {
			if (_allModeFuncList.Contains(modeFunc) == false) { _allModeFuncList.Add(modeFunc); }
		}
	}

	// 現在モード変更
	public void Select(string mode) {
		if (_modeToActiveFuncList.ContainsKey(mode) == false) {
			throw new System.ApplicationException("nothing mode");
		}

		_currentMode = mode;
		foreach(IModeFunc modeFunc in _allModeFuncList) {
			modeFunc.enabled = _modeToActiveFuncList[mode].Contains(modeFunc);
		}

		Debug.Log(_currentMode);
		this.OnSelect(_currentMode);
	}

	// 現在モード変更(前/次)
	private void SelectPrevNext(int addNum) {
		var modeList = new List<string>(_modeToActiveFuncList.Keys);

		int newIndex = modeList.IndexOf(_currentMode);
		newIndex += addNum;
		if (newIndex >= modeList.Count) { newIndex = 0;                  }
		if (newIndex < 0              ) { newIndex = modeList.Count - 1; }

		this.Select(modeList[newIndex]);
	}
	public void SelectPrev()  { this.SelectPrevNext(-1); }
	public void SelectNext()  { this.SelectPrevNext(+1); }
	public void SelectFirst()  { var modeList = new List<string>(_modeToActiveFuncList.Keys); this.Select(modeList[0]); }

}

}
