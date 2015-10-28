using rift.common;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web.Http;

namespace rift.head
{
    public class HeadController : ApiController
    {
        [Route("api/head/push")]
        [HttpPost]
        public void Push([FromBody] Parameters parameters)
        {
            if (parameters != null)
            {
                Frontier.Push(parameters.Url, null);
            }
        }
    }
}
