using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAction;
    [Header("General Settings")]
    [SerializeField] private Slider masterVolumeSld;
    [SerializeField] private Slider musicVolumeSld;
    [SerializeField] private Slider sfxVolumeSld;
    [Header("Graphics Settings")]
    [SerializeField] private TMP_InputField targetFrameRateInp;
    [SerializeField] private Toggle enablePostProcessingChk;
    [Header("Input Settings (soon)")]
    private readonly string TargetFrameRateKey = "TARGET_FRAMERATE";
    private readonly string EnablePostProcessingKey = "ENABLE_PP";
    private readonly string MasterVolumeKey = "MASTER_VOLUME";
    private readonly string MusicVolumeKey = "MUSIC_VOLUME";
    private readonly string SFXVolumeKey = "SFX_VOLUME";
    private readonly string BindingOverridesKey = "BINDINGS";

    private int targetFrameRate;
    private bool enablePostProcessing;
    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;
    public void SaveSettings() {
        PlayerPrefs.SetInt(TargetFrameRateKey, targetFrameRate);
        PlayerPrefs.SetInt(EnablePostProcessingKey, enablePostProcessing ? 1 : 0);
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume);
        PlayerPrefs.SetString(BindingOverridesKey, inputAction.SaveBindingOverridesAsJson());
    }

    public void LoadSettings() {
        targetFrameRate = PlayerPrefs.GetInt(TargetFrameRateKey, 60);
        enablePostProcessing = PlayerPrefs.GetInt(EnablePostProcessingKey, 1) == 1;
        masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        inputAction.LoadBindingOverridesFromJson(PlayerPrefs.GetString(BindingOverridesKey, ""));

        UpdateUI();
    }

    public void UpdateUI() {
        masterVolumeSld.value = masterVolume;
        musicVolumeSld.value = musicVolume;
        sfxVolumeSld.value = sfxVolume;

        targetFrameRateInp.text = targetFrameRate.ToString();
        enablePostProcessingChk.isOn = enablePostProcessing;
    }

    void OnDisable() {
        SaveSettings();
    }

    void OnEnable() {
        LoadSettings();
    }
}