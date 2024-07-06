using UnityEngine;
using System;

namespace GWK.UI {
    public class TabSwitcher : MonoBehaviour {
        [SerializeField] private ChoiceBox choiceBox;
        [SerializeField] private Button backButton;
        [SerializeField] private TabInfo[] tabs;
        private int selected = 0;
        public int Selected {
            get => selected;
            set {
                tabs[selected].tabWindow.SetActive(false);
                selected = value;
                tabs[selected].tabWindow.SetActive(true);
                choiceBox.SetSelectDown(tabs[selected].firstFocused);
                backButton.SetSelectUp(tabs[selected].lastFocused);
            }
        }
        public void SwitchTab(int idx) {
            Selected = idx;
        }
    }
    [Serializable]
    public struct TabInfo {
        public GameObject tabWindow;
        public UIElement firstFocused;
        public UIElement lastFocused;
    }
}