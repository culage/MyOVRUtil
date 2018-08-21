using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModeFunc;

public class ModeController : MonoBehaviour {

	private ModeSelector _modeSelector;
	[SerializeField] TextMesh _modeText;

	void Start () {
		_modeSelector = new ModeSelector();

		ModeChange modeChange = GetComponent<ModeChange>();
		modeChange.SetModeSelector(_modeSelector);
		var leaserPointer  = GetComponent<LeaserPointer>();
		var grabObject     = GetComponent<GrabObject>();
		var fake6dof       = GetComponent<Fake6dof>();
		var move6way       = GetComponent<Move6way>();
		var moveWarp       = GetComponent<MoveWarp>();
		var moveSegwayLike = GetComponent<MoveSegwayLike>();

		_modeSelector.AddMode("Grab"      , new IModeFunc[] { modeChange, leaserPointer, grabObject });
		_modeSelector.AddMode("MoveWarp"  , new IModeFunc[] { modeChange, leaserPointer, moveWarp });
		_modeSelector.AddMode("Move6way"  , new IModeFunc[] { modeChange, move6way });
		_modeSelector.AddMode("MoveSegway", new IModeFunc[] { modeChange, fake6dof, moveSegwayLike });
		_modeSelector.AddMode("Fake6Dof"  , new IModeFunc[] { modeChange, fake6dof });

		_modeSelector.onSelect += (mode) => {
			_modeText.text = mode;
		};

		_modeSelector.SelectFirst();
	}
}
