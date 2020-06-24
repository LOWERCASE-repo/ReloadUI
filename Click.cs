using UnityEngine;

class Click : MonoBehaviour {
	
	[SerializeField]
	private Image self;
	
	private void Update() {
		self.color = Input.GetKey(KeyCode.Mouse0) ? Color.gray : new Color(40f / 256f, 40f / 256f, 40f / 256f);
	}
}
