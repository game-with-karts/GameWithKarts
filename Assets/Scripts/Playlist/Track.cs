using System;
[Serializable]
public class Track
{
    public int sceneIdx = 1;
    public RaceSettings settings;

    public Track() {
        settings = RaceSettings.Default;
    }
}