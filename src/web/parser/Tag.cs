using System.Collections.Generic;

namespace rift.parser
{
    public class Tag
    {
        public string Name { get; set; }

        public IDictionary<string, string> Attributes { get; set; }

        public bool TrailingSlash { get; set; }
    }
}
