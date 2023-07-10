using System;
[Serializable]
public class Track
{
    public int sceneIdx = 1;
    public RaceSettings settings;

    public Track() {}
    public Track(int sceneIdx, RaceSettings settings) {
        this.sceneIdx = sceneIdx;
        this.settings = settings;
    }

}