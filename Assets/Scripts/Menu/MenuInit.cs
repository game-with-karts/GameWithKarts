using UnityEngine;

public class MenuInit : MonoBehaviour
{
    void Start() {
        SoundManager.SetMenuMusic();
        SoundManager.PlayMusic();
    }
}