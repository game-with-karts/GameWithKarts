using UnityEngine;

public sealed class Gear : MonoBehaviour {
	float t;
	float euler;

	const float rotationAmount = 50;

	void Start() {
		t = Random.Range(0f, 1f);
		euler = Random.Range(0f, 90f);
	}

	void Update() {
		t += Time.deltaTime;
		if (t >= 1f) {
			t -= 1f;
			euler += rotationAmount;
		}
		transform.rotation = Quaternion.Euler(90, 0, euler + GetValue(t) * rotationAmount);
	}

	float GetValue(float t) => t < 0.5 ? BackInOut(t * 2) : 1;

	float BackInOut(float x) {
		const float c1 = 1.70158f;
		const float c2 = c1 * 1.525f;

		return x < 0.5
		  ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
		  : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
	}
}
