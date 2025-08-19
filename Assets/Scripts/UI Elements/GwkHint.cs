using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GWK.UI {
    public class Hint : MonoBehaviour {
        [SerializeField] private TMP_Text caption;
        [SerializeField] private Image image;

        public void SetSprite(Sprite sprite) {
            image.sprite = sprite;
        }
    }
}