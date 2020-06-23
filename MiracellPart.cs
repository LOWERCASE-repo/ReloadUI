using UnityEngine;

abstract class MiracellPart : MonoBehaviour {
	
	protected Miracell miracell;
	
	virtual internal void Init(Miracell miracell) {
		this.miracell = miracell;
	}
	
	abstract internal void Eval(float time);
}
