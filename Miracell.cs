using System.Collections;
using UnityEngine;

class Miracell : MonoBehaviour {
	
	[SerializeField] [Range(2, 6)]
	private int clip = 3;
	[SerializeField]
	private float shootLockTime = 0.2f, reloadLockTime = 0.3f, chargeLockTime = 0.4f;
	[SerializeField]
	private float reloadTime = 1f, chargeTime = 2f, beamTime = 0.5f;
	
	[SerializeField]
	private Color color, baseColor;
	
	[SerializeField]
	private SpriteRenderer divider, ring, pulse, center;
	[SerializeField]
	private Transform clear;
	
	[SerializeField]
	private SpriteRenderer outerSprite, innerSprite;
	[SerializeField]
	private Transform outerAnchor, innerAnchor;
	
	private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	private SpriteRenderer[] dividers;
	private int shots;
	private bool locked, queued;
	private float chargeAng, beamAng;
	
	private void Start() {
		shots = clip;
		float gap = 360f / clip;
		chargeAng = Mathf.Floor(chargeTime) * 360f + gap / 2f;
		beamAng = gap / 2f;
		ring.sharedMaterial.SetFloat("_Start", 0f);
		SetFill(1f);
		ring.color = color;
		dividers = new SpriteRenderer[clip];
		for (int i = 0; i < clip; i++) {
			Quaternion rot = (i * gap).Rot();
			dividers[i] = Instantiate(divider, Vector3.zero, rot, transform);
			dividers[i].color = baseColor;
		}
		StartCoroutine(Reload());
	}
	
	private void Update() {
		if (queued && !locked) {
			if (shots > 0) StartCoroutine(Shoot());
			queued = false;
		}
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			if (locked) queued = true;
			else if (shots > 0) StartCoroutine(Shoot());
		}
		if (Input.GetKey(KeyCode.Mouse0) && !locked) {
			if (shots == 0) StartCoroutine(Beam());
			else if (shots < clip) StartCoroutine(Charge());
		}
	}
	
	private IEnumerator Shoot() {
		locked = true;
		float delta = 1f / clip;
		shots--;
		float start = Time.fixedTime;
		for (float i = 0f; i < shootLockTime; i = Time.fixedTime - start) {
			i = curve.Evaluate(i / shootLockTime);
			SetFill((1f + shots) / clip - i * delta);
			ScaleColor(i < 0.5f ? i * 2f : 1f - i * 2f);
			yield return new WaitForFixedUpdate();
		}
		locked = false;
	}
	
	// TODO change dividers into cells
	
	private IEnumerator Beam() {
		locked = true;
		float start = Time.fixedTime;
		center.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
		center.color = color;
		for (float i = 0f; i >= 0f;) {
			float scale = curve.Evaluate(i / beamTime) * 2f;
			if (scale < 1f) {
				Color fade = center.color;
				fade.a = scale;
				Debug.Log(fade.a);
				center.color = fade;
			} else center.color = Color.Lerp(color, color.gamma, scale - 1f);
			start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
			float delta = Time.fixedTime - start;
			i = Input.GetKey(KeyCode.Mouse0) ? i + delta : i - Mathf.PI * delta;
			i = Mathf.Clamp(i, i, beamTime);
		}
		center.color = baseColor;
		center.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
		locked = false;
		yield break;
	}
	
	private IEnumerator Reload() {
		// total = 1 / 2pi * 2 + 360
		float start = Time.fixedTime;
		for (float i = 0f; i < reloadTime; i = Time.fixedTime - start) {
			// TODO animate this, shoot
			if (locked || shots == clip) start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
		}
		locked = true;
		start = Time.fixedTime;
		for (float i = 0f; i < reloadLockTime; i = Time.fixedTime - start) {
			float time = i / reloadLockTime;
			ScaleColor(1f - time);
			ScalePulse(time);
			yield return new WaitForFixedUpdate();
		}
		ScalePulse(0f);
		SetFill(1f);
		shots = clip;
		locked = false;
		StartCoroutine(Reload());
	}
	
	private IEnumerator Charge() {
		locked = true;
		float start = Time.fixedTime;
		float fillDelta = 1f - ((float)shots / clip);
		bool cancelled = false;
		float chargeTimeSeg = chargeTime * 2f / 3f;
		for (float i = 0f; i < chargeTime && i >= 0f;) {
			ScaleColor(i / chargeTime);
			if (i < chargeTimeSeg) {
				float eval = curve.Evaluate(i / chargeTimeSeg);
				transform.rotation = (-chargeAng * eval).Rot();
				SetFill((float)shots / clip + eval * fillDelta);
			} else {
				ScaleDividers(1f - (i - chargeTimeSeg) / (chargeTime - chargeTimeSeg));
			}
			start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
			if (!Input.GetKey(KeyCode.Mouse0)) cancelled = true;
			float delta = Time.fixedTime - start;
			i = !cancelled ? i + delta : i - Mathf.PI * delta;
		}
		if (!cancelled) {
			start = Time.fixedTime;
			SetFill(0f);
			for (float i = 0f; i < chargeLockTime; i = Time.fixedTime - start) {
				ScalePulse(1f - i / chargeLockTime);
				ScaleColor(1f - i / chargeLockTime);
				yield return new WaitForFixedUpdate();
			}
			ScaleColor(0f);
			ScalePulse(0f);
			SetFill(0f);
			ScaleDividers(1f);
			shots = 0;
		}
		locked = false;
	}
	
	private void SetFill(float fill) {
		ring.sharedMaterial.SetFloat("_Fill", fill);
	}
	
	private void ScaleColor(float time) {
		time = curve.Evaluate(time);
		divider.color = baseColor;
		pulse.color = Color.Lerp(color, color.gamma, time);
		ring.color = Color.Lerp(color, color.gamma, time);
	}
	
	private void ScaleBeamColor(float time) {
		time = curve.Evaluate(time);
		foreach (SpriteRenderer divider in dividers) {
			divider.color = Color.Lerp(color, color.gamma, time);
		}
		ring.color = Color.Lerp(color, color.gamma, time);
	}
	
	private void ScalePulse(float time) {
		float scale;
		float seg = 2f / 3f;
		if (time < seg) {
			if (time < 1f / 3f) clear.localScale = new Vector3(0f, 0f, 1f);
			scale = curve.Evaluate(time / seg) * 0.8f;
			pulse.transform.localScale = new Vector3(scale, scale, 1f);
		}
		if (time >= 1f / 3f) {
			if (time >= seg) pulse.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			scale = curve.Evaluate((time - 1f / 3f) / seg) * 0.5f;
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
