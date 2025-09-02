using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public sealed class OnlineStatus : MonoBehaviour {
	private static OnlineStatus instance;
	void Awake() {
		instance = this;
		container.SetActive(false);
	}

	[SerializeField] private GameObject container;
	[SerializeField] private TMP_Text text;
	[SerializeField] private Image img;

	public static Action Done;

	private Material mat => img.material;
	
	public static void SetStatus(string message) {
		instance.mat.SetFloat("_Status", 0);
		instance.text.text = message;
		instance.container.SetActive(true);
	}

	public static void OnComplete(bool success) {
		if (!instance.container.activeInHierarchy) {
			return;
		}
		if (success) {
			instance.mat.SetFloat("_Status", 1);
			instance.text.text = "Success!";
		}
		else {
			instance.mat.SetFloat("_Status", 2);
			instance.text.text = "Something went wrong...";
		}
		Done?.Invoke();
		instance.StartCoroutine(instance.HideDelayed());
	}

	private IEnumerator HideDelayed() {
		yield return new WaitForSecondsRealtime(3);
		container.SetActive(false);
	}
}
