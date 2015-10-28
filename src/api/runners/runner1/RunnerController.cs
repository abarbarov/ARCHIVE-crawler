using rift.common;
using rift.parser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace rift.runner
{
    [RoutePrefix("api/runner")]
    public class RunnerController : RunnerControllerBase
    {
        private static readonly ConcurrentDictionary<string, Page> _cache = new ConcurrentDictionary<string, Page>();

        [Route("process")]
        [HttpPost]
        public async Task<Page> Post([FromBody] Parameters parameters)
        {
            await Cache(parameters.Url, _cache);

            Page page = _cache[parameters.Url];

            Process(page, parameters);

            return page;
        }
    }
}
