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
	private Vector3 pulseMax = new Vector3(0.8f, 0.8f, 1f);
	private int clip;
	private bool locked, queued;
	private float chargeAng;
	
	private void Start() {
		clip = size;
		float gap = 360f / size;
		chargeAng = 360f + gap / 2f;
		ring.sharedMaterial.SetFloat("_Start", 0f);
		SetFill(1f);
		ring.color = color;
		for (int i = 0; i < size; i++) {
			Instantiate(divider, Vector3.zero, (i * gap).Rot(), transform);
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
		locked = true;
		while (Input.GetKey(KeyCode.Mouse0)) {
			pulse.transform.localScale = Vector3.one;
			yield return new WaitForFixedUpdate();
		}
		pulse.transform.localScale = pulseMax;
		locked = false;
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
		clear.localScale = Vector3.zero;
		pulse.transform.localScale = Vector3.zero;
		locked = true;
		float drainLockSeg = drainLockTime / 3f;
		while (i < drainLockTime) {
			i = Time.fixedTime - start;
			float scale;
			if (i < drainLockSeg * 2f) {
				scale = curve.Evaluate(i / (drainLockSeg / 2f)) * 0.8f;
				pulse.transform.localScale = new Vector3(scale, scale, 1f);
			} else {
				clip = size;
				SetFill(1f);
				pulse.transform.localScale = pulseMax;
				scale = curve.Evaluate((i - 0.1f) / 0.2f) * 0.5f;
				clear.localScale = new Vector3(scale, scale, 1f);
			}
			pulse.color = Color.Lerp(color, color.gamma, 1f - i / 0.3f);
			yield return new WaitForFixedUpdate();
		}
		locked = false;
		pulse.color = color;
		pulse.transform.localScale = pulseMax;
		clear.localScale = pulseMax;
		StartCoroutine(Drain());
	}
	
	private IEnumerator Charge() {
		locked = true;
		float start = Time.fixedTime;
		float i = 0f;
		float fillDelta = 1f - ((float)clip / size);
		bool cancelled = false;
		while (i < chargeTime && i >= 0f) {
			if (!Input.GetKey(KeyCode.Mouse0)) cancelled = true;
			float delta = Time.fixedTime - start;
			i = !cancelled ? i + delta : i - 3f * delta;
			float eval = curve.Evaluate(i / chargeTime);
			transform.rotation = (chargeAng * eval).Rot();
			SetFill((float)clip / size + eval * fillDelta);
			ring.color = Color.Lerp(color, color.gamma, eval);
			start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
		}
		if (!cancelled) {
			start = Time.fixedTime;
			i = 0f;
			ResetPulse();
			float chargeLockSeg = chargeLockTime / 3f;
			while (i < chargeLockTime) {
				i = Time.fixedTime - start;
				float scale;
				if (i < chargeLockSeg * 2f) {
					scale = 0.5f - curve.Evaluate(i / (chargeLockSeg * 2f)) * 0.5f;
					clear.localScale = new Vector3(scale, scale, 1f);
				} else {
					SetFill(0f);
					scale = 0.8f - curve.Evaluate((i - chargeLockSeg) / (chargeLockSeg * 2f)) * 0.8f;
					pulse.transform.localScale = new Vector3(scale, scale, 1f);
				}
				pulse.color = Color.Lerp(color, color.gamma, 1f - i / 0.3f);
				yield return new WaitForFixedUpdate();
			}
			ResetPulse();
			transform.rotation = 0f.Rot();
			clip = 0; // cannot use charge to queue orb
		}
		locked = false;
	}
	
	private void SetFill(float fill) {
		ring.sharedMaterial.SetFloat("_Fill", fill);
	}
	
	private void ScrollColor(float time) {
		
	}
	
	private void ScrollPulse(float time) {
		
	}
	
	private void ResetPulse() {
		clear.localScale = pulseMax;
		pulse.transform.localScale = pulseMax;
	}
}
