using UnityEngine;

class Core : MiracellPart {
	
	[SerializeField]
	private SpriteRenderer core;
	
	override internal void Eval(float time) {
		time *= 2f;
		if (time < 1f) {
			Color fade = core.color;
			fade.a = time;
			core.color = fade;
		} else core.color = Color.Lerp(miracell.color, miracell.color.gamma, time - 1f);
	}
	// TODO add a mask to this too so that reload cuts cleanly
}
