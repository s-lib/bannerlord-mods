using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
