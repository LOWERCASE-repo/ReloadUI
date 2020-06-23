using UnityEngine;

class Pulse : MiracellPart {
	
	[SerializeField]
	private SpriteRenderer ring;
	
	override void Init(Miracell miracell) {
		this.miracell = miracell;
	}
	
	void Eval(float time) {
		ring.sharedMaterial.SetFloat("_Angle", time);
	}
}
