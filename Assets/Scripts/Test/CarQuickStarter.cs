using UnityEngine;
using UnityEngine.Audio;
using GWK.Kart;
using System.Collections.Generic;

public class CarQuickStarter : MonoBehaviour {
    public BaseCar player;
    public List<BaseCar> bots;
    public bool isAntigrav;

    public AudioMixerGroup master;
    public AudioMixerGroup music;
    public AudioMixerGroup sfx;

    void Awake() {
        
        SettingsMenu.SetVolume(master, "MasterVolume", .2f);
        SettingsMenu.SetVolume(music, "MusicVolume", PlayerPrefs.GetFloat(SettingsMenu.MusicVolumeKey));
        SettingsMenu.SetVolume(sfx, "SFXVolume", PlayerPrefs.GetFloat(SettingsMenu.SFXVolumeKey));

        player.Init(false, isAntigrav);
        player.Camera.ActivateCamera();
        player.UI.ActivateCanvas();
        bots.ForEach(b => {
            b.Init(true, isAntigrav);
            RaceManager.instance.AddCarManually(b);
        });

        RaceManager.instance.AddCarManually(player);

        player.StartRace();
    }
}
