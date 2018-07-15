using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyOVRDebug {
	// コンソール表示
	// ------------------------------------------------------------------------------------------
	private static bool _consoleInit = false;
	private static TextMesh _consoleTextMesh;
	private static string _consoleString = "";

	public static void Clear() {
		_consoleString = "";
		UpdateLog();
	}

	public static void Log(object addLine) {
		if (_consoleString != "") { _consoleString += "\n"; }
		_consoleString += System.Convert.ToString(addLine);
		UpdateLog();
	}

	private static void UpdateLog() {
		if (_consoleInit == false) {
			_consoleInit = true;
			_consoleTextMesh = GameObject.Find("Console").GetComponent<TextMesh>();
		}

		if (_consoleTextMesh == null) { return; }
		_consoleTextMesh.text = _consoleString;
	}
}
