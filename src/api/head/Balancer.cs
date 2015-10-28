using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rift.head
{
    public static class Balancer
    {
        private static readonly Dictionary<int, string> hosts = new Dictionary<int, string>{
            {1, "http://localhost:9001/api/runner/process/"},
            {2, "http://localhost:9002/api/runner/process/"},
            {3, "http://localhost:9003/api/runner/process/"}
        };

        private static Random _random = new Random();

        // TODO implement Round Robin
        public static string GetHost()
        {
            var index = _random.Next(1, 4);

            return hosts[index];
        }
    }
}
