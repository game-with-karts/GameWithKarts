using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackRegistry : MonoBehaviour
{
    public static TrackRegistry instance;
    [SerializeField] private Track[] tracks;
    public Track[] Tracks => tracks;

    void Awake()
    {
        if (instance is null) instance = this;
        else Destroy(this);
    }
}