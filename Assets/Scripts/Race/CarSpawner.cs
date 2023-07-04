using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private StartFinish startFinish;
    // TODO: make it so it uses objects representing players instead of the number of them
    public void SpawnRandom(Transform[] startPositions, PlayerSpawning spawning, int numPlayers, bool startsOnAntigrav) {
        int numBots = startPositions.Length - numPlayers;
        bool isBot;
        
        switch (spawning) {
            case PlayerSpawning.Randomly:
                int playerIndex = Random.Range(0, startPositions.Length - 1);
                for (int i = 0; i < numBots + numPlayers; i++)
                {
                    isBot = playerIndex != i;
                    SpawnCar(startPositions[i], isBot ? "Player" : "Bot", isBot, startsOnAntigrav);
                }
                break;
            case PlayerSpawning.BehindBots:
                for (int i = 0; i < numBots; i++) {
                    SpawnCar(startPositions[i], "Bot", true, startsOnAntigrav);
                }
                for (int i = numBots; i < numBots + numPlayers; i++) {
                    SpawnCar(startPositions[i], "Player", false, startsOnAntigrav);
                }
                break;
            case PlayerSpawning.AheadBots:
                for (int i = 0; i < numPlayers; i++) {
                    SpawnCar(startPositions[i], "Player", false, startsOnAntigrav);
                }
                for (int i = numPlayers; i < numBots + numPlayers; i++) {
                    SpawnCar(startPositions[i], "Bot", true, startsOnAntigrav);
                }
                break;
        }
    }

    private void SpawnCar(Transform pos, string name, bool isBot, bool startOnAntigrav) {
        GameObject car = Instantiate(carPrefab, pos.position, pos.rotation);
        car.name = name;
        BaseCar carObj = car.GetComponent<BaseCar>();
        carObj.Path.SetPath(startFinish.FirstPath);
        carObj.Init(isBot, startOnAntigrav);
    }
}