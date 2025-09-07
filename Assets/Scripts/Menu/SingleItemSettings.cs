using UnityEngine;
using GWK.UI;
using UnityEngine.UI;

public sealed class SingleItemSettings : MonoBehaviour {
    [SerializeField] private ItemType type;
    [SerializeField] private Sprite icon;
    [SerializeField] private Image image;
    [SerializeField] private CheckBox checkBox;

    public bool IsOn => checkBox.Value;
    public ItemType Type => type;

    public void Init(bool isOn) {
        checkBox.Value = isOn;
    }

    void OnEnable() {
        image.sprite = icon;
    }
}
