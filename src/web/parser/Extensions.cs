using System.Collections.Generic;
using System.Linq;

namespace rift.parser
{
    public static class Extensions
    {
        public static IEnumerable<string> Parse(this string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                yield break;
            }

            Tag tag;
            var parser = new Parser(html);
            while (parser.ParseNext("a", out tag))
            {
                string href;
                if (tag.Attributes.TryGetValue("href", out href))
                {
                    if (!string.IsNullOrEmpty(href))
                    {
                        yield return href;
                    }
                }
            }
        }

        public static IEnumerable<string> Standartize(this IEnumerable<string> hrefs, string context)
        {
            return hrefs.Select(x => x.Standartize(context));
        }
    }
}
