using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bluebottle.Base.Behaviors
{
    public static class Filter
    {
        public delegate string FilterKeyPredicate<T>(T e);

        public static string GetWildCardRegex(string filter)
        {
            return string.Format("^{0}$", Regex.Escape(filter).Replace("\\*", ".*"));
        }

        public static IEnumerable<T> FilterByWildcard<T>(IEnumerable<T> list, string inputFilter, bool ignoreCase, FilterKeyPredicate<T> predicate)
        {
            if (string.IsNullOrEmpty(inputFilter)) //nothing to filter
                return list;


            var options = RegexOptions.None;
            if (ignoreCase)
                options |= RegexOptions.IgnoreCase;

            var filterString = GetWildCardRegex(inputFilter);
            Regex filter = new Regex(filterString, options);

            var ret = list.Where((x) => { var p = predicate(x); return !string.IsNullOrEmpty(p) && filter.IsMatch(p); });

            return ret;
        }

        public static bool FilterByWildcard(string element, string inputFilter, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(inputFilter)) //nothing to filter
                return true;

            var options = RegexOptions.None;
            if (ignoreCase)
                options |= RegexOptions.IgnoreCase;

            var filterString = GetWildCardRegex(inputFilter);
            Regex filter = new Regex(filterString, options);

            return !string.IsNullOrEmpty(element) && filter.IsMatch(element); 
        }
    }
}
