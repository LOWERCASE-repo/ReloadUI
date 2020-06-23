using UnityEngine;

class Dividers : MiracellPart {
	
	[SerializeField]
	private SpriteRenderer divider;
	private SpriteRenderer[] dividers;
	
	override internal void Init(Miracell miracell) {
		this.miracell = miracell;
		float gap = 360f / miracell.clip;
		dividers = new SpriteRenderer[miracell.clip];
		for (int i = 0; i < miracell.clip; i++) {
			Quaternion rot = (i * gap).Rot();
			dividers[i] = Instantiate(divider, Vector3.zero, rot, transform);
		}
	}
	
	override internal void Eval(float time) {
		foreach (Transform child in transform) {
			child.localScale = new Vector3(0.1f * time, 0.4f, 1f);
			// TODO check if can mod x only, [0]
		}
	}
}
