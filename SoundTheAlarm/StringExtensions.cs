namespace SoundTheAlarm {
    public static class StringExtensions {
        public static bool ToBool(this string s) {
            if (!string.IsNullOrWhiteSpace(s)) {
                s = s.Trim().ToLower();
                if (s == "true") {
                    return true;
                }
            }
            return false;
        }
    }
}
