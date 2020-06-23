using UnityEngine;

class Orbitals : MiracellPart {
	
	[SerializeField]
	private SpriteRenderer outerSprite, innerSprite;
	[SerializeField]
	private Transform outerPivot, innerPivot;
	
	override internal void Eval(float time) {
		Debug.Log("orbitals" + time);
	}
}
