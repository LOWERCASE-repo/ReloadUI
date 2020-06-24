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
	private float beamWait = 0.3f, reloadWait = 1f, chargeWait = 2f;
	[SerializeField]
	internal Color color, baseColor;
	
	[Header("Parts")]
	[SerializeField]
	private MiracellPart chassis;
	[SerializeField]
	private MiracellPart chambers, bolts, orbitals, core;
	
	private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	private int shots;
	private bool locked, queued;
	
	private void Awake() {
		shots = clip;
		chassis.Init(this);
		core.Init(this);
		chambers.Init(this);
		bolts.Init(this);
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
			chambers.Scale((1f + shots) / clip - i * delta);
			// power.Scale(i < 0.5f ? i * 2f : 1f - i * 2f);
			yield return new WaitForFixedUpdate();
		}
		locked = false;
	}
	
	private IEnumerator Beam() {
		locked = true;
		float start = Time.fixedTime;
		for (float i = 0f; i >= 0f; i = Mathf.Clamp(i, i, beamWait)) {
			core.Scale(i / beamWait);
			start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
			float delta = Time.fixedTime - start;
			i = Input.GetKey(KeyCode.Mouse0) ? i + delta : i - Mathf.PI * delta;
		}
		locked = false;
		yield break;
	}
	
	private IEnumerator Reload() {
		float start = Time.fixedTime;
		for (float i = 0f; i < reloadWait; i = Time.fixedTime - start) {
			if (locked || shots == clip) start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
		}
		locked = true;
		start = Time.fixedTime;
		for (float i = 0f; i < reloadLock; i = Time.fixedTime - start) {
			i = curve.Evaluate(i / reloadLock);
			orbitals.Scale(i);
			yield return new WaitForFixedUpdate();
		}
		orbitals.Scale(0f);
		chambers.Scale(1f);
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
			// power.Scale(i / chargeWait);
			if (i < chargeWaitSeg) {
				float eval = curve.Evaluate(i / chargeWaitSeg);
				chassis.Scale(eval);
				chambers.Scale((float)shots / clip + eval * fillDelta);
			} else {
				bolts.Scale(1f - (i - chargeWaitSeg) / (chargeWait - chargeWaitSeg));
			}
			start = Time.fixedTime;
			yield return new WaitForFixedUpdate();
			if (!Input.GetKey(KeyCode.Mouse0)) cancelled = true;
			float delta = Time.fixedTime - start;
			i = !cancelled ? i + delta : i - Mathf.PI * delta;
		}
		if (!cancelled) {
			start = Time.fixedTime;
			chambers.Scale(0f);
			for (float i = 0f; i < chargeLock; i = Time.fixedTime - start) {
				i = curve.Evaluate(1f - i / chargeLock);
				orbitals.Scale(i);
				// power.Scale(i);
				yield return new WaitForFixedUpdate();
			}
			// power.Scale(0f);
			orbitals.Scale(0f);
			bolts.Scale(1f);
			shots = 0;
		}
		locked = false;
	}
}
