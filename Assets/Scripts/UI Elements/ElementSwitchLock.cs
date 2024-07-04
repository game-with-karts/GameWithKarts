namespace GWK.UI {
    public class ElementSwitchLock {
        public delegate bool LockComparison(float newValue, float lastValue);
        private float lastValue = 0;
        private readonly LockComparison comparison;
        public ElementSwitchLock(LockComparison comp) {
            comparison = comp;
        }
        public bool ShouldSwitch(float val) {
            bool result = comparison.Invoke(val, lastValue);
            lastValue = val;
            return result;
        }
    }
}
