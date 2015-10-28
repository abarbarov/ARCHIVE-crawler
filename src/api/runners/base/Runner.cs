using Microsoft.Owin.Hosting;
using rift.common;
using System;

namespace rift.runner
{
    public class Runner
    {
        private string _url;

        protected IDisposable WebApplication;

        public Runner(string url)
        {
            _url = url;
        }

        public void Start()
        {
            WebApplication = WebApp.Start<WebPipeline>(_url);
        }

        public void Stop()
        {
            WebApplication.Dispose();
        }
    }
}
