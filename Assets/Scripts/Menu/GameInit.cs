using UnityEngine;
using System.Collections.Generic;

public class GameInit : MonoBehaviour
{
    void Start() {
        print(GameRulesManager.instance);
        GameObject gameRulesObj = new();
        gameRulesObj.name = "GameRulesManager";
        gameRulesObj.AddComponent<GameRulesManager>();
    }
}