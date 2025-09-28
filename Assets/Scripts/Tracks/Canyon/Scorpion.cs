using UnityEngine;

public sealed class Scorpion : MonoBehaviour, ITrackFeature {
	private enum MoveState {
		FromStart,
		EndSpin,
		FromEnd,
		StartSpin,
	}

	[SerializeField] private Transform scorpion;
	[SerializeField] private Transform start;
	[SerializeField] private Transform end;

	[Space]
	[SerializeField] private float moveTime;
	[SerializeField] private float turnTime;

	private float t;
	private MoveState state;

	void Start() {
		RaceManager.features.Add(this);
	}

	public void ResetFeature() {
		scorpion.SetPositionAndRotation(start.position, start.rotation);
		t = 0;
		state = MoveState.FromStart;
	}

	void Update() {
		t += Time.deltaTime;
		float moveT, turnT;
		switch (state) {
			case MoveState.FromStart:
				moveT = Mathf.Clamp01(t / moveTime);
				scorpion.position = Vector3.Lerp(start.position, end.position, EaseInOut(moveT));
				if (moveT >= 1) {
					state = MoveState.EndSpin;
				}
				break;
			case MoveState.EndSpin:
				turnT = Mathf.Clamp01((t - moveTime) / turnTime);
				scorpion.rotation = Quaternion.Lerp(start.rotation, end.rotation, EaseInOut(turnT));
				if (turnT >= 1) {
					state = MoveState.FromEnd;
				}
				break;
			case MoveState.FromEnd:
				moveT = Mathf.Clamp01((t - moveTime - turnTime) / moveTime);
				scorpion.position = Vector3.Lerp(end.position, start.position, EaseInOut(moveT));
				if (moveT >= 1) {
					state = MoveState.StartSpin;
				}
				break;
			case MoveState.StartSpin:
				turnT = Mathf.Clamp01((t - 2 * moveTime - turnTime) / turnTime);
				scorpion.rotation = Quaternion.Lerp(end.rotation, start.rotation, EaseInOut(turnT));
				if (turnT >= 1) {
					state = MoveState.FromStart;
					t = 0;
				}
				break;
		}
	}

	private float EaseInOut(float x) => -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
		

	#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Gizmos.color = new Color(.5f, 1f, .5f);
		Gizmos.DrawWireCube(start.position, Vector3.one);
		Gizmos.DrawLine(start.position, end.position);
		Gizmos.DrawWireSphere(end.position, 1f);

		Gizmos.color = new Color(1f, .7f, .5f);
		Gizmos.DrawWireSphere(scorpion.position, .9f);

		Draw3DLines(start);
		Draw3DLines(end);
	}

	void Draw3DLines(Transform t) {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(t.position, t.position + t.right);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(t.position, t.position + t.up);

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(t.position, t.position + t.forward);
	}
	#endif
}
