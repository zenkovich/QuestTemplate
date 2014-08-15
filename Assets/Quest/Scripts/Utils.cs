using UnityEngine;
using System.Collections;

public static class Utils {

	public static void Reparent(this Transform transform, Transform newParent) {
		Vector3 locPos = transform.localPosition;
		Quaternion locRot = transform.localRotation;
		Vector3 locScale = transform.localScale;

		transform.parent = newParent;
		
		transform.localPosition = locPos;
		transform.localRotation = locRot;
		transform.localScale = locScale;
	}
}
