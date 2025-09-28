using UnityEngine;
using UnityEngine.Events;
using GWK.UI;

public class PlaylistRulesEditor : MonoBehaviour
{
    [SerializeField] ChoiceBox playerSpawningInput;
    [SerializeField] CheckBox spawnBots;
    [SerializeField] CheckBox cupScoring;

    public void UpdateRaceSettings(PlaylistSettings settings) {
        settings.playerSpawning = (PlayerSpawning)playerSpawningInput.Value;
        settings.spawnBots = spawnBots.Value;
        settings.cupScoring = cupScoring.Value;
    }

    public void SetDisplayFrom(PlaylistSettings settings) {
        playerSpawningInput.Value = (int)settings.playerSpawning;
        spawnBots.Value = settings.spawnBots;
        cupScoring.Value = settings.cupScoring;
    }
}
