using UnityEngine;
using System.Linq;

namespace GWK.UI {
    public class Relay : UIElement {
        [SerializeField] private UIElement[] group;
        private UIElement lastFocused;
        private UINavigationDirection lastDir;
        public override void SetFocused(UINavigationInfo info) {
            if (group.Contains(info.from)) {
                lastFocused = info.from;
                lastDir = ReverseDirection(info.direction);
                info.from = this;
                info.to = info.direction switch {
                    UINavigationDirection.Up => selectUp,
                    UINavigationDirection.Down => selectDown,
                    UINavigationDirection.Left => selectLeft,
                    UINavigationDirection.Right => selectRight,
                    _ => null,
                };
                info.to?.SetFocused(info);
                return;
            }
            
            info.from = this;
            if (info.direction == lastDir) {
                info.to = lastFocused;
                lastFocused.SetFocused(info);
                return;
            }
            info.to = group[0];
            group[0].SetFocused(info);
        }

        private UINavigationDirection ReverseDirection(UINavigationDirection dir) => dir switch {
            UINavigationDirection.Up => UINavigationDirection.Down,
            UINavigationDirection.Down => UINavigationDirection.Up,
            UINavigationDirection.Left => UINavigationDirection.Right,
            UINavigationDirection.Right => UINavigationDirection.Left,
            _ => UINavigationDirection.Up
        };
    }
}