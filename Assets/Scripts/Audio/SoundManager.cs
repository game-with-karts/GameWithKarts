using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{

    [Header("Music")]
    [SerializeField] private AudioClip mainMusic;
    [Space]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private bool playOnStart;
    [Header("UI")]
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

    private static Action OnMusicPlay;
    private static Action OnMusicStop;
    private static Action<AudioClip> OnMusicSet;
    private static Action<bool> OnMusicLoop;

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
        OnMusicPlay += MusicPlay;
        OnMusicStop += MusicStop;
        OnMusicSet += MusicSet;
        OnMusicLoop += MusicLoopable;
    }

    void Start() {
        if (playOnStart) musicSource.Play();
    }

    public static void OnHoverUI() => OnButtonHover?.Invoke();
    public static void OnConfirmUI() => OnButtonConfirm?.Invoke();
    public static void OnBackUI() => OnButtonBack?.Invoke();

    public static void PlayMusic() => OnMusicPlay?.Invoke();
    public static void StopMusic() => OnMusicStop?.Invoke();
    public static void SetMusic(AudioClip music) => OnMusicSet?.Invoke(music);
    public static void SetMusicLooping(bool isLooping) => OnMusicLoop?.Invoke(isLooping);

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

    private void MusicPlay() {
        musicSource.Play();
    }

    private void MusicStop() {
        musicSource.Stop();
    }

    private void MusicSet(AudioClip music) {
        musicSource.clip = music;
    }

    private void MusicLoopable(bool loopable) {
        musicSource.loop = loopable;
    }

    private void OnDestroy() {
        OnButtonHover -= Hover;
        OnButtonConfirm -= Confirm;
        OnButtonBack -= Back;
        OnMusicPlay -= MusicPlay;
        OnMusicStop -= MusicStop;
        OnMusicSet -= MusicSet;
        OnMusicLoop -= MusicLoopable;
    }
}