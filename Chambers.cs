using UnityEngine;
using UnityEngine.UI;

class Chambers : MiracellPart {
	
	[SerializeField]
	private Image chamber;
	private Image[] chambers;
	
	override internal void Init(Miracell miracell) {
		this.miracell = miracell;
		float delta = 360f / miracell.clip;
		chambers = new Image[miracell.clip];
		for (int i = 0; i < miracell.clip; i++) {
			Quaternion rot = (i * delta).Rot();
			chambers[i] = Instantiate(chamber, transform.position, rot, transform);
			chambers[i].fillAmount = 1f / miracell.clip;
		}
	}
	
	override internal void Scale(float time) {
		int index = (int)Mathf.Clamp(time * miracell.clip, 0f, miracell.clip - 1f);
		float delta = time * miracell.clip - index;
		Debug.Log(time + " " + index + " " + delta);
		chambers[index].fillAmount = 1f / miracell.clip * delta;
	}
	
	void ScaleSingle(int index, float time) {
		
	}
	
	void Reverse(bool reverse) {
		
	}
	
	void Reset(bool full) {
		
	}
}
