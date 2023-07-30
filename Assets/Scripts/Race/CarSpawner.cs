using UnityEngine;
using System.Collections.Generic;
using System;

using URandom = UnityEngine.Random;
using SRandom = System.Random;
public class CarSpawner : MonoBehaviour
{
    [SerializeField] private BaseCar carPrefab;
    [SerializeField] private StartFinish startFinish;
    [Space]
    [SerializeField] private CarStats playerStats;
    [SerializeField] private CarStats botStats;

    private RaceSettings settings;
    private CarBuilder carBuilder;
    private Transform[] startPositions;
    public BaseCar[] SpawnRandom(Transform[] startPositions, RaceSettings settings, List<PlayerInfo> players, bool startsOnAntigrav) {
        List<PlayerInfo> playersOnly = players.FindAll(x => x.IsPlayer);
        List<PlayerInfo> botsOnly = players.FindAll(x => !x.IsPlayer);
        int numCars = GameRulesManager.currentTrack.settings.spawnBots ? players.Count : playersOnly.Count;
        BaseCar[] cars = new BaseCar[numCars];
        this.settings = settings;
        this.startPositions = startPositions;
        int i = 0;
        if (!settings.spawnBots) {
            PickRandom(playersOnly, cars, startsOnAntigrav, ref i);
            return cars;
        }
        switch (settings.playerSpawning) {
            case PlayerSpawning.Randomly:
                PickRandom(players, cars, startsOnAntigrav, ref i);
                break;
            case PlayerSpawning.BehindBots:
                PickRandom(botsOnly, cars, startsOnAntigrav, ref i);
                PickRandom(playersOnly, cars, startsOnAntigrav, ref i);
                break;
            case PlayerSpawning.AheadBots:
                PickRandom(playersOnly, cars, startsOnAntigrav, ref i);
                PickRandom(botsOnly, cars, startsOnAntigrav, ref i);
                break;
        }
        return cars;
    }

    private void PickRandom(List<PlayerInfo> players, BaseCar[] cars, bool startOnntigrav, ref int i) {
        int range = players.Count - 1;
        int idx;
        for (int x = 0; x < players.Count; x++)
        {
            idx = URandom.Range(0, range);
            cars[i] = SpawnCar(startPositions[i], players[idx], startOnntigrav);
            (players[idx], players[range]) = (players[range], players[idx]);
            range--;
            i++;
        }
    }

    private BaseCar SpawnCar(Transform pos, PlayerInfo player, bool startOnAntigrav) {
        bool isBot = !player.IsPlayer;
        return new CarBuilder(carPrefab, pos, player.Name)
                          .IsBot(isBot)
                          .SetStats(isBot ? botStats : playerStats)
                          .StartOnAntigrav(startOnAntigrav)
                          .SetPath(startFinish.FirstPath)
                          .SetNumberOfLaps(settings.numberOfLaps)
                          .Build();
    }
}