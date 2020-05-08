using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using WePrint.Data;

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

        public static IHtmlContent Lines<T>(this IHtmlHelper<T> helper, string? s)
        {
            return helper.Raw(s?.Replace("\n", "<br \\>"));
        }

        public static string ShortId(this IIdentifiable<Guid> entity)
        {
            return entity.Id.ToString().Substring(0, 8);
        }

        public static string Sanitize(this string input)
        {
            return HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(input));
        }
    }
}
