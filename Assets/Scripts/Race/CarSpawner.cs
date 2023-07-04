using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private StartFinish startFinish;
    // TODO: make it so it uses objects representing players instead of the number of them
    public void SpawnRandom(Transform[] startPositions, RaceSettings settings, int numPlayers, bool startsOnAntigrav) {
        int numBots = startPositions.Length - numPlayers;
        bool isBot;
        
        switch (settings.playerSpawning) {
            case PlayerSpawning.Randomly:
                int playerIndex = Random.Range(0, startPositions.Length - 1);
                for (int i = 0; i < numBots + numPlayers; i++)
                {
                    isBot = playerIndex != i;
                    SpawnCar(startPositions[i], isBot ? "Bot" : "Player", isBot, startsOnAntigrav, settings);
                }
                break;
            case PlayerSpawning.BehindBots:
                for (int i = 0; i < numBots; i++) {
                    SpawnCar(startPositions[i], "Bot", true, startsOnAntigrav, settings);
                }
                for (int i = numBots; i < numBots + numPlayers; i++) {
                    SpawnCar(startPositions[i], "Player", false, startsOnAntigrav, settings);
                }
                break;
            case PlayerSpawning.AheadBots:
                for (int i = 0; i < numPlayers; i++) {
                    SpawnCar(startPositions[i], "Player", false, startsOnAntigrav, settings);
                }
                for (int i = numPlayers; i < numBots + numPlayers; i++) {
                    SpawnCar(startPositions[i], "Bot", true, startsOnAntigrav, settings);
                }
                break;
        }
    }

    private void SpawnCar(Transform pos, string name, bool isBot, bool startOnAntigrav, RaceSettings settings) {
        GameObject car = Instantiate(carPrefab, pos.position, pos.rotation);
        car.name = name;
        BaseCar carObj = car.GetComponent<BaseCar>();
        carObj.Path.SetPath(startFinish.FirstPath);
        carObj.Init(isBot, startOnAntigrav);
        carObj.Path.numLaps = settings.numberOfLaps;
    }
}