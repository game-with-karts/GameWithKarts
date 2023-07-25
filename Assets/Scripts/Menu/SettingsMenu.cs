using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAction;
    [Header("General Settings")]
    [SerializeField] private Slider masterVolumeSld;
    [SerializeField] private Slider musicVolumeSld;
    [SerializeField] private Slider sfxVolumeSld;
    [Space]
    [SerializeField] private AudioMixerGroup masterMixGroup;
    [SerializeField] private AudioMixerGroup musicMixGroup;
    [SerializeField] private AudioMixerGroup sfxMixGroup;
    [Header("Graphics Settings")]
    [SerializeField] private TMP_InputField targetFrameRateInp;
    [SerializeField] private Toggle enablePostProcessingChk;
    [Header("Input Settings (soon)")]
    public static readonly string TargetFrameRateKey = "TARGET_FRAMERATE";
    public static readonly string EnablePostProcessingKey = "ENABLE_PP";
    public static readonly string MasterVolumeKey = "MASTER_VOLUME";
    public static readonly string MusicVolumeKey = "MUSIC_VOLUME";
    public static readonly string SFXVolumeKey = "SFX_VOLUME";
    public static readonly string BindingOverridesKey = "BINDINGS";

    private int targetFrameRate;
    private bool enablePostProcessing;
    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;
    public void SaveSettings() {
        PlayerPrefs.SetInt(TargetFrameRateKey, int.Parse(targetFrameRateInp.text));
        PlayerPrefs.SetInt(EnablePostProcessingKey, enablePostProcessingChk.isOn ? 1 : 0);
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolumeSld.value);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolumeSld.value);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolumeSld.value);
        PlayerPrefs.SetString(BindingOverridesKey, inputAction.SaveBindingOverridesAsJson());
    }

    public void LoadSettings() {
        targetFrameRate = PlayerPrefs.GetInt(TargetFrameRateKey, 60);
        enablePostProcessing = PlayerPrefs.GetInt(EnablePostProcessingKey, 1) == 1;
        masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, .7f);
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        inputAction.LoadBindingOverridesFromJson(PlayerPrefs.GetString(BindingOverridesKey, ""));
        UpdateSettings();
        UpdateUI();
    }

    public void UpdateUI() {
        masterVolumeSld.value = masterVolume;
        musicVolumeSld.value = musicVolume;
        sfxVolumeSld.value = sfxVolume;

        targetFrameRateInp.text = targetFrameRate.ToString();
        enablePostProcessingChk.isOn = enablePostProcessing;
    }

    public void UpdateKeybinds() {
        // this doesn't make sense, but it might work
        inputAction.LoadBindingOverridesFromJson(inputAction.SaveBindingOverridesAsJson());
    }

    public void UpdateSettings() {
        SetVolume(masterMixGroup, "MasterVolume", masterVolumeSld.value);
        SetVolume(musicMixGroup, "MusicVolume", musicVolumeSld.value);
        SetVolume(sfxMixGroup, "SFXVolume", sfxVolumeSld.value);
        Application.targetFrameRate = int.Parse(targetFrameRateInp.text);
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