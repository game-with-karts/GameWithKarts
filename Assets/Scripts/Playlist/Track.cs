using System;
[Serializable]
public class Track
{
    public string levelName = string.Empty;
    public RaceSettings settings;

    public Track() {}
    public Track(string levelName, RaceSettings settings) {
        this.levelName = levelName;
        this.settings = settings;
    }

}
