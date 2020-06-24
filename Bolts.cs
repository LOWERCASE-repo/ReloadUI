using UnityEngine;
using UnityEngine.UI;

class Bolts : MiracellPart {
	
	[SerializeField]
	private Transform bolt;
	
	override internal void Init(Miracell miracell) {
		this.miracell = miracell;
		float delta = 360f / miracell.clip;
		for (int i = 0; i < miracell.clip; i++) {
			Quaternion rot = (i * delta).Rot();
			Instantiate(bolt, transform.position, rot, transform);
		}
	}
	
	override internal void Scale(float time) {
		foreach (Transform child in transform) {
			child.localScale = new Vector3(0.1f * time, 0.4f, 1f);
			// TODO check if can mod x only, [0]
		}
	}
}
