using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Thumbnails", menuName = "Track Thumbnails")]
public class TrackThumbnails : ScriptableObject {
    public List<TrackThumbnailEntry> thumbnails;

    public TrackThumbnailEntry this[int i] => thumbnails[i];

    public int Length => thumbnails.Count;
}

[Serializable]
public struct TrackThumbnailEntry {
    public string levelName;
    public Sprite thumbnail;

    public Sprite image => thumbnail;
    public string caption => levelName;
}
