using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace rift.common
{
    public class Page
    {
        public string Url { get; set; }

        public string Header { get; set; }

        public string Html { get; set; }

        public DateTime Timestamp { get; set; }

        public HttpStatusCode ResponseCode { get; set; }
    }
}
