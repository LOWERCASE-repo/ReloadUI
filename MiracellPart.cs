using UnityEngine;

abstract class MiracellPart : MonoBehaviour {
	
	protected Miracell miracell;
	
	void Init(Miracell miracell) {
		this.miracell = miracell;
	}
	
	abstract void Eval(float time);
}
