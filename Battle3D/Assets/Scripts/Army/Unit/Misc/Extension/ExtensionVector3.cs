using System;
using UnityEngine;

public static class ExtensionVector3 {
	public static Vector3 PosOpposite(this Vector3 pos1, Vector3 pos2) {
		return new Vector3(pos1.x - pos1.x, pos1.y, pos1.z - pos2.z);
	}

	public static bool isInCircle(this Vector3 pos1, int radius, Vector3 pos2) {
		return Math.Pow(pos2.x - pos1.x, 2) + Math.Pow(pos2.z - pos1.z, 2) < Math.Pow(radius, 2);
	}

	public static int Distance(this Vector3 pos1, Vector3 pos2) {
		return Mathf.FloorToInt(
			Mathf.Sqrt(Mathf.Pow(pos1.x - pos2.x, 2) + Mathf.Pow(pos1.z - pos2.z, 2))
			);
	}

	public static int CompartUnitDistance(this Vector3 pos, Unit x, Unit y) {
		if (x == null) {
			if (y == null) {		// If x is null and y is null, they're equal. 
				return 0;
			} else {				// If x is null and y is not null, y is greater.
				return -1;
			}
		} else {					// If x is not null...
			if (y == null) {		// ...and y is null, x is greater.
				return 1;
			} else {				// ...and y is not null, compare the distance of the two unit.
				var retval = x.Tr.position.Distance(pos).CompareTo(y.Tr.position.Distance(pos));

				if (retval != 0) {	// If the unit are not of equal length, the longer string is greater.
					return retval;
				} else {			// If the strings are of equal length, sort them with ordinary string comparison.
					return x.CompareLifeTo(y);
				}
			}
		}
	}
}