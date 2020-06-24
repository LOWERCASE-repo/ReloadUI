using UnityEngine;
using UnityEngine.UI;

class Core : MiracellPart {
	
	[SerializeField]
	private Image core;
	
	override internal void Scale(float time) {
		time *= 2f;
		if (time < 1f) core.color = Color.Lerp(miracell.baseColor, miracell.color, time);
		else core.color = Color.Lerp(miracell.color, miracell.color.gamma, time - 1f);
	}
}
