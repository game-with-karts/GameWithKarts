using UnityEngine;
using System;
using TMPro;
using GWK.UI;

public sealed class LeaderboardScreen : MonoBehaviour {
	[SerializeField] private GameObject loginScreen;
	[SerializeField] private GameObject settingsScreen;
	[Space]
	[SerializeField] private GameObject loginError;
	[SerializeField] private TextInputBox usernameInput;
	[SerializeField] private TextInputBox passwordInput;
	[Space]
	[SerializeField] private TMP_Text settingsUsername;

	private string username => JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("LoginInfo")).username;

	void OnEnable() {
		Leaderboard.OnLogin += LoginHandler;
		Leaderboard.OnRegister += LoginHandler;
	}
	void OnDisable() {
		Leaderboard.OnLogin -= LoginHandler;
		Leaderboard.OnRegister -= LoginHandler;
	}

	public void Open() {
		if (PlayerPrefs.GetString("LoginInfo") == string.Empty) {
			loginScreen.SetActive(true);
			loginError.SetActive(false);
			return;
		}
		settingsUsername.text = $"Logged in as {username}";
		settingsScreen.SetActive(true);
	}

	public void Login() => Leaderboard.Login(usernameInput.Text, passwordInput.Text);

	public void Register() => Leaderboard.Register(usernameInput.Text, passwordInput.Text);

	public void Clear() => PlayerPrefs.SetString("Records", string.Empty);

	public void SendAll() => Leaderboard.SubmitAllTimes();

	public void Retrieve() => Leaderboard.RetrieveRecords();

	public void Logout() {
		PlayerPrefs.SetString("LoginInfo", string.Empty);
		settingsScreen.SetActive(false);
		loginScreen.SetActive(true);
	}

	void LoginHandler(bool success) {
		if (!success) {
			loginError.SetActive(true);
			return;
		}
		if (loginScreen.activeInHierarchy) {
			loginScreen.SetActive(false);
			settingsUsername.text = $"Logged in as {username}";
			settingsScreen.SetActive(true);
		}
	}
}
