using System.Collections;
using UnityEngine;

// TODO shove this all in a namespace

class Miracell : MonoBehaviour {
	
	[Header("Stats")]
	[SerializeField] [Range(2, 6)]
	internal int clip = 3;
	[SerializeField]
	private float shootLock = 0.2f, reloadLock = 0.3f, chargeLock = 0.4f;
	[SerializeField]
	private float beamWait = 0.5f, reloadWait = 1f, chargeWait = 2f;
	[SerializeField]
	internal Color color, baseColor;
	
	[Header("Parts")]
	[SerializeField]
	private MiracellPart pivot;
	[SerializeField]
	private MiracellPart core, ring, dividers, power;
	
	private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	private int shots;
	private bool locked, queued;
	
	private void Awake() {
		shots = clip;
		pivot.Init(this);
		core.Init(this);
		ring.Init(this);
		dividers.Init(this);
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
		for (float i = 0f; i < shootLock; i = Time.fixedTime - start) {
			i = curve.Evaluate(i / shootLock);
			ring.Eval((1f + shots) / clip - i * delta);
			power.Eval(i < 0.5f ? i * 2f : 1f - i * 2f);
			yield return new WaitForFixedUpdate();
		}
		locked = false;
	}
	
	private IEnumerator Beam() {
		locked = true;
		float start = Time.fixedTime;
		center.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
		center.color = color;
		for (float i = 0f; i >= 0f;) {
			
			start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
			float delta = Time.fixedTime - start;
			i = Input.GetKey(KeyCode.Mouse0) ? i + delta : i - Mathf.PI * delta;
			i = Mathf.Clamp(i, i, beamWait);
		}
		center.color = baseColor;
		center.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
		locked = false;
		yield break;
	}
	
	private IEnumerator Reload() {
		// total = 1 / 2pi * 2 + 360
		float start = Time.fixedTime;
		for (float i = 0f; i < reloadWait; i = Time.fixedTime - start) {
			// TODO animate this, shoot
			if (locked || shots == clip) start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
		}
		locked = true;
		start = Time.fixedTime;
		for (float i = 0f; i < reloadLock; i = Time.fixedTime - start) {
			float time = i / reloadLock;
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
		float chargeWaitSeg = chargeWait * 2f / 3f;
		for (float i = 0f; i < chargeWait && i >= 0f;) {
			ScaleColor(i / chargeWait);
			if (i < chargeWaitSeg) {
				float eval = curve.Evaluate(i / chargeWaitSeg);
				transform.rotation = (-chargeAng * eval).Rot();
				SetFill((float)shots / clip + eval * fillDelta);
			} else {
				ScaleDividers(1f - (i - chargeWaitSeg) / (chargeWait - chargeWaitSeg));
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
			for (float i = 0f; i < chargeLock; i = Time.fixedTime - start) {
				ScalePulse(1f - i / chargeLock);
				ScaleColor(1f - i / chargeLock);
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
}
