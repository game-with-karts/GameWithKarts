using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

namespace GWK.UI {
    public class ScrollableListEntry : MonoBehaviour {
        [SerializeField] private Image bg;
        [Header("Track Name")]
        [SerializeField] private Image trackImg;
        [SerializeField] private TMP_Text trackName;
        [Header("Colours")]
        [SerializeField] private Color colorDefault;
        [SerializeField] private Color colorSelected;
        [SerializeField] private Color colorUnfocused;
        [Header("Info")]
        [SerializeField] private TMP_Text lapCount;
        [SerializeField] private TMP_Text raceMode;
        [SerializeField] private Image mirrorMode;
        [SerializeField] private Image itemless;

        public RectTransform rectTransform => transform as RectTransform;
        private Color targetColor;
        private ScrollableList parent;

        void Awake() {
            trackImg.material = new(trackImg.material);
        }

        public void Init(ScrollableList parent) => this.parent = parent;

        public void SetInfo(string name, Sprite trackThumbnail, RaceSettings settings) {
            trackName.text = name;
            trackImg.material.SetTexture("_Image", trackThumbnail.texture);
            lapCount.text = $"{settings.numberOfLaps} lap{(settings.numberOfLaps == 1 ? "" : "s")}";
            raceMode.text = PlaylistEditor.GetRaceModeString(settings.raceMode);
            mirrorMode.enabled = settings.mirrorMode;
            itemless.enabled = !settings.useItems;
        }
        
        public void SetSelected(bool selected, bool focused) {
            if (selected) {
                if (focused) {
                    targetColor = colorSelected;
                    return;
                }
                targetColor = colorUnfocused;
                return;
            }
            targetColor = colorDefault;
        }

        void Update() {
            bg.color = Color.Lerp(bg.color, targetColor, 10 * Time.unscaledDeltaTime);
        }

        public void OnHover() {
            parent.Select(this);
        }

        public void OnClick() {
            parent.Choose();
        }
    }
}