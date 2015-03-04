using UnityEngine;

public class TimeSpeed : MonoBehaviour {
	[SerializeField, Range(0.1f, 1f)]	private const float _ralenti	= 0.5f;
	[SerializeField, Range(1f, 2f)]		private const float _accelere	= 1.5f;

	void Update() {
		if (Input.GetKeyDown(KeyCode.R)) {
            Time.timeScale *= _ralenti;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
		} else if (Input.GetKeyDown(KeyCode.Y)) {
			if ((Time.timeScale * _accelere) < 100f)
				Time.timeScale *= _accelere;
			else
				Time.timeScale = 100f;
			Time.fixedDeltaTime = 0.02F * Time.timeScale;
		} else if (Input.GetKeyDown(KeyCode.T)) {
			Time.timeScale = 1.0F;
			Time.fixedDeltaTime = 0.02F * Time.timeScale;
		}
    }
}