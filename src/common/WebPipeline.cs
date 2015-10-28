using Owin;
using System.Web.Http;

namespace rift.common
{
    public class WebPipeline
    {
        public void Configuration(IAppBuilder application)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            application.UseWebApi(config);
        }
    }
}
