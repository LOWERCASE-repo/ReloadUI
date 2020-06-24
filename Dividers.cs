using UnityEngine;
using UnityEngine.UI;

class Dividers : MiracellPart {
	
	[SerializeField]
	private Transform divider;
	
	override internal void Init(Miracell miracell) {
		this.miracell = miracell;
		float gap = 360f / miracell.clip;
		dividers = new Transform[miracell.clip];
		for (int i = 0; i < miracell.clip; i++) {
			Quaternion rot = (i * gap).Rot();
		}
	}
	
	override internal void Scale(float time) {
		foreach (Transform child in transform) {
			child.localScale = new Vector3(0.1f * time, 0.4f, 1f);
			// TODO check if can mod x only, [0]
		}
	}
}
