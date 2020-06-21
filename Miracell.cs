using System.Collections;
using UnityEngine;

class Miracell : MonoBehaviour {
	
	[SerializeField]
	private SpriteRenderer divider, ring, pulse;
	[SerializeField]
	private Transform clear;
	
	[SerializeField] [Range(2, 6)]
	private int size = 3;
	[SerializeField]
	private float orbLockTime = 0.2f, drainLockTime = 0.3f, chargeLockTime = 0.4f;
	[SerializeField]
	private float drainTime = 1f, chargeTime = 2f, beamTime = 0.2f;
	[SerializeField]
	private Color color;
	
	private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	private SpriteRenderer[] dividers;
	private int clip;
	private bool locked, queued;
	private float chargeAng;
	
	private void Start() {
		clip = size;
		float gap = 360f / size;
		chargeAng = 720f + gap / 2f;
		ring.sharedMaterial.SetFloat("_Start", 0f);
		SetFill(1f);
		ring.color = color;
		dividers = new SpriteRenderer[size];
		for (int i = 0; i < size; i++) {
			Quaternion rot = (i * gap).Rot();
			dividers[i] = Instantiate(divider, Vector3.zero, rot, transform);
		}
		StartCoroutine(Drain());
	}
	
	private void Update() {
		if (queued && !locked) {
			if (clip > 0) StartCoroutine(Orb());
			queued = false;
		}
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			if (locked) queued = true;
			else if (clip > 0) StartCoroutine(Orb());
		}
		if (Input.GetKey(KeyCode.Mouse0) && !locked) {
			if (clip == 0) StartCoroutine(Beam());
			else if (clip < size) StartCoroutine(Charge());
		}
	}
	
	private IEnumerator Orb() {
		locked = true;
		clip--;
		float delta = 1f / size;
		float start = Time.fixedTime;
		float i = 0f;
		while (i < 1f) {
			i = (Time.fixedTime - start) / orbLockTime;
			SetFill((1f + clip) / size - curve.Evaluate(i) * delta);
			ring.color = Color.Lerp(color, color.gamma, i < 0.5f ? i : 1f - i);
			yield return new WaitForFixedUpdate();
		}
		ring.color = color;
		SetFill((float)clip / size);
		locked = false;
	}
	
	private IEnumerator Beam() {
		// locked = true;
		// while (Input.GetKey(KeyCode.Mouse0)) {
		// 	pulse.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
		// 	yield return new WaitForFixedUpdate();
		// }
		// ScalePulse(0f);
		// locked = false;
		yield break;
	}
	
	private IEnumerator Drain() {
		float start = Time.fixedTime;
		float i = 0f;
		while (i < drainTime) {
			if (locked || clip == size) start = Time.fixedTime;
			i = Time.fixedTime - start;
			yield return new WaitForFixedUpdate();
		}
		start = Time.fixedTime;
		i = 0f;
		locked = true;
		while (i < drainLockTime) {
			i = Time.fixedTime - start;
			float time = i / drainLockTime;
			ScaleColor(1f - time);
			ScalePulse(time);
			yield return new WaitForFixedUpdate();
		}
		ScalePulse(0f);
		SetFill(1f);
		clip = size;
		locked = false;
		StartCoroutine(Drain());
	}
	
	private IEnumerator Charge() {
		locked = true;
		float start = Time.fixedTime;
		float i = 0f;
		float fillDelta = 1f - ((float)clip / size);
		bool cancelled = false;
		float chargeTimeSeg = chargeTime - chargeLockTime;
		while (i < chargeTime && i >= 0f) {
			if (!Input.GetKey(KeyCode.Mouse0)) cancelled = true;
			float delta = Time.fixedTime - start;
			i = !cancelled ? i + delta : i - Mathf.PI * delta;
			ScaleColor(i / chargeTime);
			if (i < chargeTimeSeg) {
				float eval = curve.Evaluate(i / chargeTimeSeg);
				transform.rotation = (chargeAng * eval).Rot();
				SetFill((float)clip / size + eval * fillDelta);
			} else {
				ScaleDividers(1f - (i - chargeTimeSeg) / (chargeTime - chargeTimeSeg));
			}
			start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
		}
		if (!cancelled) {
			start = Time.fixedTime;
			i = 0f;
			SetFill(0f);
			while (i < chargeLockTime) {
				i = Time.fixedTime - start;
				ScalePulse(1f - i / chargeLockTime);
				ScaleColor(1f - i / chargeLockTime);
				yield return new WaitForFixedUpdate();
			}
			ScaleColor(0f);
			ScalePulse(0f);
			SetFill(0f);
			ScaleDividers(1f);
			clip = 0;
		}
		transform.rotation = 0f.Rot();
		locked = false;
	}
	
	private void SetFill(float fill) {
		ring.sharedMaterial.SetFloat("_Fill", fill);
	}
	
	private void ScaleColor(float time) {
		time = curve.Evaluate(time);
		pulse.color = Color.Lerp(color, color.gamma, time);
		ring.color = Color.Lerp(color, color.gamma, time);
	}
	
	private void ScalePulse(float time) {
		float scale;
		if (time < 2f / 3f) {
			clear.localScale = new Vector3(0f, 0f, 1f);
			scale = curve.Evaluate(time / 2f * 3f) * 0.8f;
			pulse.transform.localScale = new Vector3(scale, scale, 1f);
		} else {
			pulse.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			scale = curve.Evaluate((time - 1f / 3f) / 2f * 3f) * 0.5f;
			clear.localScale = new Vector3(scale, scale, 1f);
		}
	}
	
	private void ScaleDividers(float time) {
		time = curve.Evaluate(time);
		foreach (SpriteRenderer divider in dividers) {
			divider.transform.localScale = new Vector3(0.1f * time, 0.4f, 1f);
		}
	}
}
