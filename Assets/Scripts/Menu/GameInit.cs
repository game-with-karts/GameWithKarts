using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private LevelLoader loader;
    void Start() {
        if (GameRulesManager.instance is not null) {
            GameRulesManager.instance.players = null;
            return;
        }
        GameObject gameRulesObj = new();
        gameRulesObj.name = "GameRulesManager";
        gameRulesObj.AddComponent<GameRulesManager>();
        loader.LoadLevel();
    }
}