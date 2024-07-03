using UnityEngine;
using UnityEngine.Events;

namespace GWK.UI {

    public class UIElement : MonoBehaviour {
        protected Window window;
        protected bool focused => ReferenceEquals(window.CurrentFocused, this);
        public UnityEvent OnFocusGained;
        public UnityEvent OnFocusLost;
        public void Init(Window win) {
            window = win;
            Debug.Log($"Init done! {gameObject.name}");
        }

        public void SetFocused() {
            if (focused) {
                return;
            }
            window.SetFocused(this);
            OnFocusGained.Invoke();
        }

        public void SetUnfocused() {
            if (!focused) {
                return;
            }
            OnFocusLost.Invoke();
        }

    }
}

