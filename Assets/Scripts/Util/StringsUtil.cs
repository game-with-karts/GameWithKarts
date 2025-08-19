namespace GWK.Util {
    public static class StringsUtil {
        public static string FormatPlace(int place) {
            string suffix;
            if ((place / 10) % 10 != 1) {
                suffix = (place % 10) switch {
                    1 => "st",
                    2 => "nd",
                    3 => "rd",
                    _ => "th"
                };
            }
            else suffix = "th";
            return $"{place}{suffix}";
        }

        public static string GetFormattedTime(int time, bool forceMinutes = true) {
            if (time == int.MaxValue) return "--:--.---";
            int seconds = time / 1000;

            int m = seconds / 60;
            int s = seconds % 60;
            int ms = time % 1000;
            string m_str = string.Format("{0:00}", m);
            string s_str;
            if (forceMinutes || m >= 1) {
                s_str = string.Format("{0:00}", s);
            }
            else {
                s_str = $"{s}";
            }
            string ms_str = string.Format("{0:000}", ms);
            if (forceMinutes) {
                return $"{m_str}:{s_str}.{ms_str}";
            }
            return $"{s_str}.{ms_str}";
        }
    }
}