using UnityEngine;
using UnityEngine.Audio;

public class GameInit : MonoBehaviour
{
    [SerializeField] private LevelLoader loader;
    [SerializeField] private AudioMixerGroup masterGroup;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    void Start() {
        SettingsMenu.SetVolume(masterGroup, "MasterVolume", PlayerPrefs.GetFloat(SettingsMenu.MasterVolumeKey));
        SettingsMenu.SetVolume(musicGroup, "MusicVolume", PlayerPrefs.GetFloat(SettingsMenu.MusicVolumeKey));
        SettingsMenu.SetVolume(sfxGroup, "SFXVolume", PlayerPrefs.GetFloat(SettingsMenu.SFXVolumeKey));
        loader.LoadLevel();
    }
}