using UnityEngine;

class Click : MonoBehaviour {
	
	[SerializeField]
	private SpriteRenderer self;
	
	private void Update() {
		self.color = Input.GetKey(KeyCode.Mouse0) ? new Color(184f / 256f, 21f / 256f, 91f / 256f) : new Color(40f / 256f, 40f / 256f, 40f / 256f);
	}
}
