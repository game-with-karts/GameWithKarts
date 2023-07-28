using UnityEngine;
using System.Collections.Generic;
public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private StartFinish startFinish;

    private RaceSettings settings;
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
            idx = Random.Range(0, range);
            cars[i] = SpawnCar(startPositions[i], players[idx], startOnntigrav);
            (players[idx], players[range]) = (players[range], players[idx]);
            range--;
            i++;
        }
    }

    private BaseCar SpawnCar(Transform pos, PlayerInfo player, bool startOnAntigrav) {
        GameObject car = Instantiate(carPrefab, pos.position, pos.rotation);
        car.name = player.Name;
        BaseCar carObj = car.GetComponent<BaseCar>();
        carObj.Path.SetPath(startFinish.FirstPath);
        carObj.Init(!player.IsPlayer, startOnAntigrav);
        carObj.Path.numLaps = settings.numberOfLaps;
        return carObj;
    }
}