using UnityEngine;

public class Scope {
	private readonly int _min;
	private readonly int _max;
	private Unit _me;

	public Scope(int min, int max) {
		_min = min;
		_max = max;
	}

	public int IsInRangeOf(Vector3 pos1, Vector3 pos2) {
		if (pos1.isInCircle(_max, pos2)) {
			if (!pos1.isInCircle(_min, pos2)) {
				return 0;
			} else {
				return -1;
			}
		} else {
			return 1;
		}
	}
}
