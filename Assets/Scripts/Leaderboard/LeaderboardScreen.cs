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

	private string username => PlayerPrefs.GetString("Username");

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

	public void Login() {
		OnlineStatus.SetStatus("Logging in...");
		Leaderboard.OnLogin += OnlineStatus.OnComplete;
		OnlineStatus.Done = () => Leaderboard.OnLogin -= OnlineStatus.OnComplete;
		Leaderboard.Login(usernameInput.Text, passwordInput.Text);
	}

	public void Register() {
		OnlineStatus.SetStatus("Registering...");
		Leaderboard.OnRegister += OnlineStatus.OnComplete;
		OnlineStatus.Done = () => Leaderboard.OnRegister -= OnlineStatus.OnComplete;
		Leaderboard.Register(usernameInput.Text, passwordInput.Text);
	}

	public void Clear() => PlayerPrefs.SetString("Records", string.Empty);

	public void SendAll() {
		OnlineStatus.SetStatus("Sending...");
		Leaderboard.OnSubmit += OnlineStatus.OnComplete;
		OnlineStatus.Done = () => Leaderboard.OnSubmit -= OnlineStatus.OnComplete;
		Leaderboard.SubmitAllTimes();
	}

	public void Retrieve() {
		OnlineStatus.SetStatus("Retrieving...");
		Leaderboard.OnRetrieve += OnlineStatus.OnComplete;
		OnlineStatus.Done = () => Leaderboard.OnRetrieve -= OnlineStatus.OnComplete;
		Leaderboard.RetrieveRecords();
	}

	public void Logout() {
		PlayerPrefs.SetString("LoginInfo", string.Empty);
		PlayerPrefs.SetString("Username", string.Empty);
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
