using Newtonsoft.Json;
using rift.common;
using rift.common.logger;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace rift.head
{
    public class MessageSender
    {
        private const int LOOP_DELAY = 1000;

        private const int MAX_RUNNING_TASKS = 100;

        //private const int MAX_IDLE_TIME = 10000;

        private CancellationTokenSource _token;

        private ActionBlock<DateTimeOffset> _task;

        private string _root;

        private ILogger _logger = new Logger();

        //private Stopwatch _stopwatch = new Stopwatch();

        private ActionBlock<DateTimeOffset> Loop(Func<DateTimeOffset, CancellationToken, Task> action, CancellationToken cancellationToken)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ActionBlock<DateTimeOffset> block = null;

            block = new ActionBlock<DateTimeOffset>(async now =>
            {
                await action(now, cancellationToken).ConfigureAwait(false);
                await Task.Delay(LOOP_DELAY, cancellationToken).ConfigureAwait(false);
                block.Post(DateTimeOffset.Now);
            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            return block;
        }

        public void Start(string root)
        {
            _token = new CancellationTokenSource();
            _task = Loop((now, ct) => SendMessagesAsync(ct), _token.Token);
            _task.Post(DateTimeOffset.Now);
            _root = root;
            //_stopwatch.Start();

            Frontier.Push(_root, null);
        }

        public void Stop()
        {
            using (_token)
            {
                _token.Cancel();
            }

            _token = null;
            _task = null;
        }

        private Task<Page> SendAsync(string url, CancellationToken token)
        {
            return Task<Page>.Run(async () =>
            {
                var client = new HttpClient();
                var host = Balancer.GetHost();
                HttpResponseMessage response = await client.PostAsync<Parameters>(host, new Parameters { Root = _root, Url = url }, new JsonMediaTypeFormatter(), token);

                return JsonConvert.DeserializeObject<Page>(await response.Content.ReadAsStringAsync());
            }, token);
        }

        private Task SendMessagesAsync(CancellationToken token)
        {
            //if (_stopwatch.ElapsedMilliseconds > MAX_IDLE_TIME)
            //{
            //    _token.Cancel();
            //    return null;
            //}

            //_stopwatch.Reset();
            var tasks = new ConcurrentQueue<Task<Page>>();
            var map = new ConcurrentDictionary<int, string>();
            var running = 0;

            while (!Frontier.IsEmpty() && running < MAX_RUNNING_TASKS)
            {
                Interlocked.Increment(ref running);
                var url = Frontier.Pop();

                if (!string.IsNullOrEmpty(url))
                {
                    var task = SendAsync(url, token);
                    map.AddOrUpdate(task.Id, url, (id, u) => url);
                    tasks.Enqueue(task);
                }
            }

            return Task.Factory.ContinueWhenAll(tasks.ToArray(), list =>
            {
                foreach (var t in list)
                {
                    switch (t.Status)
                    {
                        case TaskStatus.Faulted:
                            Frontier.Push(map[t.Id], null);
                            _logger.Error(t.Exception);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(t.Exception.InnerException.Message);
                            break;
                        case TaskStatus.RanToCompletion:
                            Frontier.Push(map[t.Id], t.Result);
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(t.Exception);
                            break;
                    }
                }
            }, token);
        }
    }
}
