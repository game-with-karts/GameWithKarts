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
	// updated every time a track or kart physics change
	const int VERSION_ID = 1;
	private static readonly string server = "http://frog03.mikr.us:20577";

	public static void Register(string username, string password) {
		instance.StartCoroutine(instance.RegisterAsync(username, password));
	}

	private IEnumerator RegisterAsync(string username, string password) {
		string data = JsonUtility.ToJson(new LoginRequest() {username = username, password = password});
		using (UnityWebRequest req = UnityWebRequest.Post(
					$"{server}/register", 
					data,
					"application/json")) {
			yield return req.SendWebRequest();
			bool success = req.responseCode >= 200 && req.responseCode < 300;
			if (success) {
				LoginInfo info = JsonUtility.FromJson<LoginInfo>(req.downloadHandler.text);
				PlayerPrefs.SetString(Constants.LOGIN_INFO_KEY, info.token);
				PlayerPrefs.SetString(Constants.USERNAME_KEY, info.username);
				PlayerPrefs.SetString(Constants.LOGIN_REQUEST_KEY, data);
			}
			OnRegister?.Invoke(success);
		}
	}

	public static void Login(string username, string password) {
		instance.StartCoroutine(instance.LoginAsync(username, password));
	}

	private IEnumerator LoginAsync(string username, string password) {
		string data = JsonUtility.ToJson(new LoginRequest() {username = username, password = password});
		using (UnityWebRequest req = UnityWebRequest.Post(
					$"{server}/login", 
					data,
					"application/json")) {
			yield return req.SendWebRequest();
			bool success = req.responseCode >= 200 && req.responseCode < 300;
			if (success) {
				LoginInfo info = JsonUtility.FromJson<LoginInfo>(req.downloadHandler.text);
				PlayerPrefs.SetString(Constants.LOGIN_INFO_KEY, info.token);
				PlayerPrefs.SetString(Constants.USERNAME_KEY, info.username);
				PlayerPrefs.SetString(Constants.LOGIN_REQUEST_KEY, data);
			}
			OnLogin?.Invoke(success);
		}
	}

	public static void SubmitTime(TimeRecord record) {
		if (!PlayerPrefs.HasKey(Constants.LOGIN_INFO_KEY)) {
			return;
		}
		List<TimeRecord> ltr = new();
		ltr.Add(record);
		instance.StartCoroutine(instance.SubmitTimesAsync(ltr));
	}

	public static void SubmitAllTimes() {
		if (!PlayerPrefs.HasKey(Constants.LOGIN_INFO_KEY)) {
			return;
		}
		instance.StartCoroutine(instance.SubmitTimesAsync(new List<TimeRecord>()));
	}

	private IEnumerator SubmitTimesAsync(List<TimeRecord> records) {
		Records recordsObj = new();
		string jwt = PlayerPrefs.GetString(Constants.LOGIN_INFO_KEY);
		if (records.Count == 0) {
			recordsObj.records = JsonUtility.FromJson<Records>(PlayerPrefs.GetString(Constants.RECORDS_KEY)).records;
		}
		else {
			recordsObj.records = records;
		}
		recordsObj.version = VERSION_ID;
		string sentData = JsonUtility.ToJson(recordsObj);
		using (UnityWebRequest req = UnityWebRequest.Post($"{server}/a/submit", sentData, "application/json")) {
			req.SetRequestHeader("Authorization", $"Bearer {jwt}");
			yield return req.SendWebRequest();
			OnSubmit?.Invoke(req.responseCode >= 200 && req.responseCode < 300);
		}
	}

	public static void RetrieveRecords() {
		if (!PlayerPrefs.HasKey(Constants.LOGIN_INFO_KEY)) {
			return;
		}
		instance.StartCoroutine(instance.RetrieveRecordsAsync());
	}

	private IEnumerator RetrieveRecordsAsync() {
		string jwt = PlayerPrefs.GetString(Constants.LOGIN_INFO_KEY);
		using (UnityWebRequest req = UnityWebRequest.Get($"{server}/a/records/{VERSION_ID}")) {
			req.SetRequestHeader("Authorization", $"Bearer {jwt}");
			yield return req.SendWebRequest();
			bool success = req.responseCode >= 200 && req.responseCode < 300;
			if (success) {
				PlayerPrefs.SetString(Constants.RECORDS_KEY, req.downloadHandler.text);
			}
			OnRetrieve?.Invoke(success);
		}
	}
}
