using UnityEngine;

class Ring : MiracellPart {
	
	[SerializeField]
	private SpriteRenderer ring;
	
	override internal void Init(Miracell miracell) {
		this.miracell = miracell;
	}
	
	override internal void Eval(float time) {
		ring.sharedMaterial.SetFloat("_Angle", time);
	}
}
