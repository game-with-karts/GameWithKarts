public class PlayerInfo {
    private string name;
    private bool isPlayer;

    public string Name => name;
    public bool IsPlayer => isPlayer;

    public PlayerInfo(string name, bool isPlayer) {
        this.name = name;
        this.isPlayer = isPlayer;
    }

}