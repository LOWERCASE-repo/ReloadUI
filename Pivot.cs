using UnityEngine;

class Pivot : MiracellPart {
	
	private float angle;
	
	override internal void Init(Miracell miracell) {
		this.miracell = miracell;
		float gap = 360f / miracell.clip;
		angle = 720f + gap / 2f;
	}
	
	override internal void Eval(float time) {
		transform.rotation = (angle * time).Rot();
	}
}
