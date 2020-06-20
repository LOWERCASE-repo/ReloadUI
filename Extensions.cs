using UnityEngine;

internal static class Extensions {
	
	internal static Vector2 Rotate(this Vector2 dir, float angle) {
		return Quaternion.AngleAxis(angle, Vector3.forward) * dir;
	}
	
	internal static Quaternion Rot(this float angle) {
		return Quaternion.AngleAxis(angle, Vector3.forward);
	}
	
	// internal static Vector2 pos(this MonoBehaviour script) {
	// 	return (Vector2)script.transform.position;
	// }
	
	internal static Vector2 Predict(this Vector2 relPos, Vector2 relVel, float speed) {
		float a = speed * speed - relVel.sqrMagnitude;
		float b = Vector2.Dot(relPos, relVel);
		float det = b * b + a * relPos.sqrMagnitude;
		if (det < 0f) return relPos;
		det = Mathf.Sqrt(det);
		float timeA = b - det;
		float timeB = b + det;
		if (timeA > 0f) return relPos + timeA * relVel / a;
		if (timeB > 0f) return relPos + timeB * relVel / a;
		return relPos;
	}
}
