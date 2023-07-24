using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

public class UIAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip hoverSFX;
    [SerializeField] private AudioClip confirmSFX;
    [SerializeField] private AudioClip backSFX;
    [Space]
    [SerializeField] private AudioMixerGroup mixerGroup;
    private List<AudioSource> sources;
    private int sourceIdx = 0;

    private static Action OnButtonHover;
    private static Action OnButtonConfirm;
    private static Action OnButtonBack;
    private static Action OnButtonClick;
    private static Action OnButtonDisabledClick;

    void Awake() {
        sources = new();
        for (int i = 0; i < 4; i++) {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = mixerGroup;
            sources.Add(src);
        }
        OnButtonHover += Hover;
        OnButtonConfirm += Confirm;
        OnButtonBack += Back;
    }

    public static void OnHover() => OnButtonHover?.Invoke();
    public static void OnConfirm() => OnButtonConfirm?.Invoke();
    public static void OnBack() => OnButtonBack?.Invoke();

    private void Hover() {
        SetAndPlay(hoverSFX);
    }

    private void Back() {
        SetAndPlay(backSFX);
    }

    private void Confirm() {
        SetAndPlay(confirmSFX);
    }

    private void SetAndPlay(AudioClip clip) {
        sourceIdx = (sourceIdx + 1) % sources.Count;
        sources[sourceIdx].clip = clip;
        sources[sourceIdx].Play();
    }

    private void OnDestroy() {
        OnButtonHover -= Hover;
        OnButtonConfirm -= Confirm;
        OnButtonBack -= Back;
    }
}