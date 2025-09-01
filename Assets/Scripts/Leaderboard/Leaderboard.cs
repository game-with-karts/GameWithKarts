using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using UnityEngine;
using UnityEngine.Networking;

public class Leaderboard : MonoBehaviour {
	void Awake() {
		instance = this;
	}

	private static Leaderboard instance;

	public static event Action<bool> OnRegister;
	public static event Action<bool> OnLogin;
	public static event Action<bool> OnSubmit;
	public static event Action<bool> OnRetrieve;
	#if UNITY_EDITOR
	private static readonly string server = "http://127.0.0.1:20577";
	#else
	private static readonly string server = "http://frog03.mikr.us:20577";
	#endif

	public static void Register(string username, string password) {
		instance.StartCoroutine(instance.RegisterAsync(username, password));
	}

	private IEnumerator RegisterAsync(string username, string password) {
		string data = JsonUtility.ToJson(new LoginInfo() {username = username, password = password});
		using (UnityWebRequest req = UnityWebRequest.Post(
					$"{server}/register", 
					data,
					"application/json")) {
			yield return req.SendWebRequest();
			bool success = req.responseCode >= 200 && req.responseCode < 300;
			if (success) {
				PlayerPrefs.SetString("LoginInfo", req.downloadHandler.text);
			}
			OnRegister?.Invoke(success);
		}
	}

	public static void Login(string username, string password) {
		instance.StartCoroutine(instance.LoginAsync(username, password));
	}

	private IEnumerator LoginAsync(string username, string password) {
		using (UnityWebRequest req = UnityWebRequest.Post(
					$"{server}/login", 
					JsonUtility.ToJson(new LoginInfo() {username = username, password = password}), 
					"application/json")) {
			yield return req.SendWebRequest();
			bool success = req.responseCode >= 200 && req.responseCode < 300;
			if (success) {
				PlayerPrefs.SetString("LoginInfo", req.downloadHandler.text);
			}
			OnLogin?.Invoke(success);
		}
	}

	public static void SubmitTime(TimeRecord record) {
		if (!PlayerPrefs.HasKey("LoginInfo")) {
			return;
		}
		List<TimeRecord> ltr = new();
		ltr.Add(record);
		instance.StartCoroutine(instance.SubmitTimesAsync(ltr));
	}

	public static void SubmitAllTimes() {
		if (!PlayerPrefs.HasKey("LoginInfo")) {
			return;
		}
		instance.StartCoroutine(instance.SubmitTimesAsync(new List<TimeRecord>()));
	}

	private IEnumerator SubmitTimesAsync(List<TimeRecord> records) {
		Records recordsObj = new();
		UserData userData = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("LoginInfo"));
		recordsObj.user_id = userData.id;
		if (records.Count == 0) {
			// take from player prefs
			recordsObj.records = JsonUtility.FromJson<Records>(PlayerPrefs.GetString("Records")).records;
		}
		else {
			recordsObj.records = records;
		}
		using (UnityWebRequest req = UnityWebRequest.Post($"{server}/submit", JsonUtility.ToJson(recordsObj), "application/json")) {
			yield return req.SendWebRequest();
			OnSubmit?.Invoke(req.responseCode >= 200 && req.responseCode < 300);
		}
	}

	public static void RetrieveRecords() {
		if (!PlayerPrefs.HasKey("LoginInfo")) {
			return;
		}
		instance.StartCoroutine(instance.RetrieveRecordsAsync());
	}

	private IEnumerator RetrieveRecordsAsync() {
		UserData userData = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("LoginInfo"));
		using (UnityWebRequest req = UnityWebRequest.Get($"{server}/records/{userData.id}")) {
			yield return req.SendWebRequest();
			bool success = req.responseCode >= 200 && req.responseCode < 300;
			if (success) {
				PlayerPrefs.SetString("Records", req.downloadHandler.text);
			}
			OnRetrieve?.Invoke(success);
		}
	}
}
