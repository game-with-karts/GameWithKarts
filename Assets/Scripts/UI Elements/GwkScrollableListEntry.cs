using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GWK.UI {
    public class ScrollableListEntry : MonoBehaviour {
        [SerializeField] private Image bg;
        [Header("Track Name")]
        [SerializeField] private Image trackImg;
        [SerializeField] private TMP_Text trackName;
        [Header("Colours")]
        [SerializeField] private Color colorDefault;
        [SerializeField] private Color colorSelected;
        [Header("Info")]
        [SerializeField] private TMP_Text lapCount;
        [SerializeField] private TMP_Text raceMode;

        public RectTransform rectTransform => transform as RectTransform;

        void Awake() {
        }

        public void SetInfo(string name, Sprite trackThumbnail, RaceSettings settings) {
            trackName.text = name;
            trackImg.material.SetTexture("_Image", trackThumbnail.texture);
        }
        
        public void SetSelected(bool selected) {
            bg.color = selected ? colorSelected : colorDefault;
        }
    }
}