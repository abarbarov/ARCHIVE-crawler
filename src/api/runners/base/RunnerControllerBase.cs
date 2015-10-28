using rift.common;
using rift.common.logger;
using rift.parser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace rift.runner
{
    public abstract class RunnerControllerBase : ApiController
    {
        private const string _filter = ".*(\\.(css|js|bmp|gif|jpe?g|png|tiff?|mid|mp2|mp3|mp4|wav|avi|mov|mpeg|ram|m4v|pdf|rm|smil|wmv|swf|wma|zip|rar|gz))$";

        private const string _host = "http://localhost:9000/api/head/push";

        private static ILogger _logger = new Logger();

        private static List<Task> _tasks = new List<Task>();

        private static SemaphoreSlim _throttle;

        HttpClient _client = new HttpClient();

        protected async void Process(Page page, Parameters parameters)
        {
            var cts = new CancellationTokenSource();
            ServicePointManager.DefaultConnectionLimit = 10;
            ServicePointManager.MaxServicePointIdleTime = 100;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.CheckCertificateRevocationList = false;

            _throttle = new SemaphoreSlim(initialCount: 24);

            await _throttle.WaitAsync();

            page.Html.Parse()
            .Standartize(parameters.Root)
            .Where(url => url.StartsWith(parameters.Root) && !Regex.Match(url, _filter).Success)
            .AsParallel()
            .ForAll(url =>
                _tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var response = await _client.PostAsync<Parameters>(_host, new Parameters { Url = url }, new JsonMediaTypeFormatter(), cts.Token).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        cts.Cancel();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        _throttle.Release();
                    }
                })));
        }

        protected static async Task Cache(string url, ConcurrentDictionary<string, Page> cache)
        {
            if (!cache.ContainsKey(url))
            {
                Page page = await Downloader.DownloadAsync(url);
                cache.TryAdd(url, page);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Downloaded: {0}", url);
            }
        }
    }
}
