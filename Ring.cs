using UnityEngine;
using UnityEngine.UI;

class Ring : MiracellPart {
	
	[SerializeField]
	private Image ring;
	
	override internal void Init(Miracell miracell) {
		this.miracell = miracell;
	}
	
	override internal void Scale(float time) {
		ring.sharedMaterial.SetFloat("_Angle", time);
	}
}
