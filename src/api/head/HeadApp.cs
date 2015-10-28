using Microsoft.Owin.Hosting;
using rift.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rift.head
{
    public class HeadApp
    {
        protected IDisposable WebApplication;
        public void Start()
        {
            WebApplication = WebApp.Start<WebPipeline>("http://localhost:9000");
            new MessageSender().Start("http://www.npo-es.ru/");
        }

        public void Stop()
        {
            WebApplication.Dispose();
        }
    }
}
