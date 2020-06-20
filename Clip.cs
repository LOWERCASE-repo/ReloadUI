using System.Collections;
using UnityEngine;

class Clip : MonoBehaviour {
	
	[SerializeField]
	private SpriteRenderer divider, ring, pulse;
	[SerializeField]
	private Transform clear;
	
	[SerializeField] [Range(2, 6)]
	private int size = 3;
	[SerializeField]
	private float fireTime = 0.2f, chargeTime = 2f, reloadTime = 1f;
	[SerializeField]
	private Color regular, charged;
	
	private int clip;
	
	private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	
	private bool queued;
	private bool locked;
	
	private void Start() {
		clip = size;
		float gap = 360f / size;
		float shift = gap / 2f;
		// ring.sharedMaterial.SetFloat("_Start", shift);
		ring.sharedMaterial.SetFloat("_Start", 0f);
		SetFill(1f);
		ring.color = regular;
		for (int i = 0; i < size; i++) {
			// Instantiate(divider, Vector3.zero, (i * gap + shift).Rot(), transform);
			Instantiate(divider, Vector3.zero, (i * gap).Rot(), transform);
		}
		StartCoroutine(Reload());
	}
	
	private void Update() {
		if (queued && !locked) {
			if (clip > 0) StartCoroutine(Orb());
			else StartCoroutine(Beam());
		}
		if (clip > 0) {
			if (queued && !locked) {
				StartCoroutine(Orb());
				queued = false;
			}
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				if (locked) queued = true;
				else StartCoroutine(Orb());
			}
		} else {
			if (!locked && Input.GetKeyDown(KeyCode.Mouse0)) {
				StartCoroutine(Beam());
			}
		}
	}
	
	private IEnumerator Orb() {
		locked = true;
		clip--;
		float delta = 1f / size;
		float start = Time.fixedTime;
		float i = 0f;
		while (i < 1f) {
			i = (Time.fixedTime - start) / fireTime;
			SetFill((1f + clip) / size - curve.Evaluate(i) * delta);
			ring.color = Color.Lerp(regular, charged, i < 0.5f ? i : 1f - i);
			yield return new WaitForFixedUpdate();
		}
		ring.color = regular;
		SetFill((float)clip / size);
		locked = false;
	}
	
	private IEnumerator Beam() {
		locked = true;
		while (Input.GetKey(KeyCode.Mouse0)) {
			pulse.transform.localScale = Vector3.one;
			yield return new WaitForFixedUpdate();
		}
		pulse.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		locked = false;
	}
	
	private IEnumerator Reload() {
		float start = Time.fixedTime;
		float i = 0f;
		while (i < reloadTime) {
			if (locked || clip == size) start = Time.fixedTime;
			i = Time.fixedTime - start;
			yield return new WaitForFixedUpdate();
		}
		start = Time.fixedTime;
		i = 0f;
		clear.localScale = Vector3.zero;
		pulse.transform.localScale = Vector3.zero;
		while (i < 0.3f) {
			i = Time.fixedTime - start;
			float scale;
			if (i < 0.2f) {
				scale = curve.Evaluate(i / 0.2f) * 0.8f;
				pulse.transform.localScale = new Vector3(scale, scale, 1f);
			} else {
				clip = size;
				SetFill(1f);
				pulse.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			}
			if (i >= 0.1f) {
				scale = curve.Evaluate((i - 0.1f) / 0.2f) * 0.5f;
				clear.localScale = new Vector3(scale, scale, 1f);
			}
			pulse.color = Color.Lerp(regular, charged, 1f - i / 0.3f);
			yield return new WaitForFixedUpdate();
		}
		pulse.color = regular;
		pulse.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		clear.localScale = new Vector3(0.8f, 0.8f, 1f);
		StartCoroutine(Reload());
	}
	
	// private IEnumerator Charge() {
	// 	float start = Time.fixedTime;
	// 	float i = 0f;
	// 	while (i < chargeTime) {
	// 		if (locked || clip == size || ) start = Time.fixedTime;
	// 		i = Time.fixedTime - start;
	// 		yield return new WaitForFixedUpdate();
	// 	}
	// }
	
	private void SetFill(float fill) {
		ring.sharedMaterial.SetFloat("_Fill", fill);
	}
}
