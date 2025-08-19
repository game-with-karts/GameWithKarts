using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using GWK.UI;
using System;

public class SettingsMenu : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private Slider masterVolumeSld;
    [SerializeField] private Slider musicVolumeSld;
    [SerializeField] private Slider sfxVolumeSld;
    [Space]
    [SerializeField] private AudioMixerGroup masterMixGroup;
    [SerializeField] private AudioMixerGroup musicMixGroup;
    [SerializeField] private AudioMixerGroup sfxMixGroup;
    [Header("Graphics Settings")]
    [SerializeField] private CheckBox fpsCounterChk;
    [SerializeField] private NumberInputBox targetFrameRateInp;
    [SerializeField] private CheckBox enablePostProcessingChk;
    [Header("Input Settings (soon)")]
    [SerializeField] private InputActionAsset inputAction;
    public static readonly string TargetFrameRateKey = "TARGET_FRAMERATE";
    public static readonly string EnablePostProcessingKey = "ENABLE_PP";
    public static readonly string MasterVolumeKey = "MASTER_VOLUME";
    public static readonly string MusicVolumeKey = "MUSIC_VOLUME";
    public static readonly string SFXVolumeKey = "SFX_VOLUME";
    public static readonly string BindingOverridesKey = "BINDINGS";
    public static readonly string FPSCounterKey = "FPS_COUNTER";

    public static event Action OnSettingsUpdated;

    private int targetFrameRate;
    private bool enablePostProcessing;
    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;
    private bool showFPSCounter;
    public void SaveSettings() {
        PlayerPrefs.SetInt(TargetFrameRateKey, targetFrameRateInp.Value);
        PlayerPrefs.SetInt(EnablePostProcessingKey, enablePostProcessingChk.Value ? 1 : 0);
        PlayerPrefs.SetInt(FPSCounterKey, fpsCounterChk.Value ? 1 : 0);
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolumeSld.Value);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolumeSld.Value);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolumeSld.Value);
        PlayerPrefs.SetString(BindingOverridesKey, inputAction.SaveBindingOverridesAsJson());
        OnSettingsUpdated?.Invoke();
    }

    public void LoadSettings() {
        targetFrameRate = PlayerPrefs.GetInt(TargetFrameRateKey, 60);
        enablePostProcessing = PlayerPrefs.GetInt(EnablePostProcessingKey, 1) == 1;
        masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, .7f);
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        showFPSCounter = PlayerPrefs.GetInt(FPSCounterKey, 0) == 1;
        inputAction.LoadBindingOverridesFromJson(PlayerPrefs.GetString(BindingOverridesKey, ""));
        UpdateSettings();
        UpdateUI();
    }

    public void UpdateUI() {
        masterVolumeSld.Value = masterVolume;
        musicVolumeSld.Value = musicVolume;
        sfxVolumeSld.Value = sfxVolume;

        targetFrameRateInp.Value = targetFrameRate;
        enablePostProcessingChk.Value = enablePostProcessing;
        fpsCounterChk.Value = showFPSCounter;
    }

    // public void UpdateKeybinds() {
    //     // this doesn't make sense, but it might work
    //     string overrides = inputAction.SaveBindingOverridesAsJson();
    // }

    public void UpdateSettings() {
        SetVolume(masterMixGroup, "MasterVolume", masterVolumeSld.Value);
        SetVolume(musicMixGroup, "MusicVolume", musicVolumeSld.Value);
        SetVolume(sfxMixGroup, "SFXVolume", sfxVolumeSld.Value);
        Application.targetFrameRate = targetFrameRateInp.Value;
    }

    public static void SetVolume(AudioMixerGroup group, string name, float value) {
        float volume = value == 0 ? -80 : 20 * Mathf.Log10(value);
        group.audioMixer.SetFloat(name, volume);
    }

    void OnDisable() {
        SaveSettings();
    }

    void OnEnable() {
        LoadSettings();
    }
}