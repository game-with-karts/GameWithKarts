using UnityEngine;

public class PlayerInfo {
    private string name;
    private bool isPlayer;
    private Color color;

    public string Name => name;
    public bool IsPlayer => isPlayer;
    public Color Color => color;

    public PlayerInfo(string name, bool isPlayer, Color color) {
        this.name = name;
        this.isPlayer = isPlayer;
        this.color = color;
    }

}
