using UnityEngine;

class Pulse : MiracellPart {
	
	[SerializeField]
	private SpriteRenderer pulse;
	[SerializeField]
	private Transform clear;
	
	override internal void Eval(float time) {
		float scale;
		float smallSeg = 1f / 3f, largeSeg = 2f / 3f;
		if (time < largeSeg) {
			if (time < smallSeg) clear.localScale = new Vector3(0f, 0f, 1f);
			scale = time / largeSeg * 0.8f;
			pulse.transform.localScale = new Vector3(scale, scale, 1f);
		}
		if (time >= smallSeg) {
			if (time >= largeSeg) pulse.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			scale = (time - smallSeg) / largeSeg * 0.5f;
			clear.localScale = new Vector3(scale, scale, 1f);
		}
		// pulse.color = Color.Lerp(miracell.color, miracell.color.gamma, time);
	}
}
