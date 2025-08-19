using System.Linq;

namespace GWK.UI {
    public class Container : UIElement {
        public override void Init(Window win) {
            base.Init(win);
            GetComponentsInChildren<UIElement>().Where(e => e != this).ToList().ForEach(e => e.Init(win));
        }
    }
}