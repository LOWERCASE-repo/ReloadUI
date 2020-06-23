using UnityEngine;

class Orbitals : MiracellPart {
	
	private float angle;
	
	override void Init(Miracell miracell) {
		base.Init();
		float gap = 360f / miracell.clip;
		chargeAng = 720f + gap / 2f;
	}
	
	void Eval(float time) {
		transform.rotation = (angle * time).Rot();
	}
}
