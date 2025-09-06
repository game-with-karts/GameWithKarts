using UnityEngine;
using UnityEngine.Audio;

public class GameInit : MonoBehaviour
{
    [SerializeField] private LevelLoader loader;
    [SerializeField] private AudioMixerGroup masterGroup;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    void Start() {
        SettingsMenu.SetVolume(masterGroup, "MasterVolume", PlayerPrefs.GetFloat(SettingsMenu.MasterVolumeKey, .7f));
        SettingsMenu.SetVolume(musicGroup, "MusicVolume", PlayerPrefs.GetFloat(SettingsMenu.MusicVolumeKey, 1));
        SettingsMenu.SetVolume(sfxGroup, "SFXVolume", PlayerPrefs.GetFloat(SettingsMenu.SFXVolumeKey, 1));

        Leaderboard.OnLogin += OnLogin;

        if (PlayerPrefs.HasKey("LoginRequest")) {
            LoginRequest req = JsonUtility.FromJson<LoginRequest>(PlayerPrefs.GetString("LoginRequest"));
            Leaderboard.Login(req.username, req.password);
        }
        else {
            loader.LoadLevel();
        }
    }

    void OnLogin(bool b) {
        Leaderboard.OnLogin -= OnLogin;
        loader.LoadLevel();
    } 
}
