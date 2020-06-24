using UnityEngine;
using UnityEngine.UI;

class Orbitals : MiracellPart {
	
	[SerializeField]
	private Image outerSprite, innerSprite;
	[SerializeField]
	private Transform outerPivot, innerPivot;
	
	override internal void Scale(float time) {
	}
}
