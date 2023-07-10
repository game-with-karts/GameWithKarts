using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlaylistEditorTrackEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text indexDisplay;
    [SerializeField] private TMP_Text trackName;
    [SerializeField] private TMP_Text raceMode;
    [SerializeField] private Image img;
    [HideInInspector] public int index;
    [HideInInspector] public PlaylistEditor editor;

    public void Select() {
        editor.SelectForEditing(index);
    }

    public TMP_Text IndexDisplay => indexDisplay;
    public TMP_Text NameDisplay => trackName;
    public TMP_Text RaceModeDisplay => raceMode;
    public float height => (transform as RectTransform).sizeDelta.y;
    public Color color {
        get => img.color;
        set => img.color = value;
    }
}