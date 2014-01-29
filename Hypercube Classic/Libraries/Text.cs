using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Hypercube_Classic.Libraries {
    class Text {
        const string RegexString = "[^A-Za-z0-9!\\^\\~$%&/()=?{}\t\\[\\]\\\\ ,\\\";.:\\-_#'+*<>|@]|&.$|&.(&.)";

        /// <summary>
        /// Replaces invalid chat characters with "*".
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string CleanseString(string Input) {
            var Matcher = new Regex(RegexString, RegexOptions.Multiline);
            return Matcher.Replace(Input, "*");
        }

        /// <summary>
        /// Returns true if an illegal character is inside of the given string.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool StringMatches(string Input) {
            var Matcher = new Regex(RegexString, RegexOptions.Multiline);
            return Matcher.IsMatch(Input);
        }
    }
}
