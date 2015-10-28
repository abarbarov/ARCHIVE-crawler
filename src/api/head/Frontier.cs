using rift.common;
using rift.common.concurrent;
using rift.common.logger;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace rift.head
{
    public class Frontier
    {
        private static readonly ConcurrentDistinctQueue<string> _queue = new ConcurrentDistinctQueue<string>();

        private static ConcurrentDictionary<string, Page> _frontier = new ConcurrentDictionary<string, Page>();

        private static ILogger _logger = new Logger();

        public static void Push(string url, Page page)
        {
            if (_frontier.ContainsKey(url) && page != null)
            {
                return;
            }

            if (page != null)
            {
                _frontier.AddOrUpdate(url, page, (key, p) => page);
                _logger.Trace(string.Format("{0}", url));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("pushed: {0} :{1}", url, _frontier.Count);

                return;
            }

            if (!_frontier.ContainsKey(url))
            {
                _queue.TryAdd(url);
            }
        }

        public static string Pop()
        {
            string url;
            if (_queue.TryTake(out url))
            {
                return url;
            }

            return null;
        }

        public static bool IsEmpty()
        {
            return _queue.Count == 0;
        }
    }
}
