using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WePrint.Utilities
{
    public static class Extensions
    {
        private static readonly Regex FileNameReplace = new Regex("[^\\w\\d\\.]");
        private static readonly Regex WhitespaceReplace = new Regex("\\s");

        public static string SafeFileName(this string file)
        {
            var nameOnly = Path.GetFileName(file);
            var noWhite = WhitespaceReplace.Replace(nameOnly, "_");
            var safe = FileNameReplace.Replace(noWhite, "");
            return safe;
        }
    }
}
