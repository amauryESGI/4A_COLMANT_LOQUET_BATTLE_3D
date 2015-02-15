using System;
using UnityEngine;

public static class ExtensionVector3 {
	//public static bool isSuperieur(this Vector3 pos1, Vector3 pos2) {
	//	return pos1.x > pos2.x && pos1.y > pos2.y;
	//}

	//public static bool isInferieur(this Vector3 pos1, Vector3 pos2) {
	//	return pos1.x < pos2.x && pos1.y < pos2.y;
	//}

	public static Vector3 PosOpposite(this Vector3 pos1, Vector3 pos2) {
		return new Vector3(pos1.x - pos1.x, pos1.y, pos1.z - pos2.z);
	}

	public static bool isInCircle(this Vector3 pos1, int radius, Vector3 pos2) {
		return Math.Pow(pos2.x - pos1.x, 2) + Math.Pow(pos2.y - pos1.y, 2) < Math.Pow(radius, 2);
	}

	public static int Distance(this Vector3 pos1, Vector3 pos2) {
		return Mathf.FloorToInt(
			Mathf.Sqrt(Mathf.Pow(pos1.x - pos2.x, 2) + Mathf.Pow(pos1.y - pos2.y, 2))
			);
	}
}