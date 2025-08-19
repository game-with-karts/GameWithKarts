using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GWK.UI {
    public class HintDict : MonoBehaviour {
        public static HintDict instance { get; private set; }

        [SerializeField] private List<IconRepository> icons;

        void Awake() {
            if (instance is not null) {
                Destroy(gameObject);
                return;
            }
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public IconRepository GetIcons(string key) => icons.Where(i => i.key == key).Single();
    }

    [Serializable]
    public struct IconRepository {
        public string key;
        public Sprite confirm;
        public Sprite cancel;
        public Sprite alternative;
        public Sprite upDown;
        public Sprite leftRight;
        public Sprite tabsL;
        public Sprite tabsR;
    }
}