using rift.common.logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace rift.common
{
    public class Downloader
    {
        private static readonly ILogger _logger = new Logger();
        public static async Task<Page> DownloadAsync(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AllowAutoRedirect = true;
            webRequest.UserAgent = UserAgent.IE11.Get();

            try
            {
                using (var response = await webRequest.GetResponseAsync())
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)response;
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream(), true))
                    {
                        string html = await streamReader.ReadToEndAsync();
                        return new Page { Url = url, Html = html, Timestamp = DateTime.UtcNow, ResponseCode = httpWebResponse.StatusCode };
                    }
                }
            }
            catch (WebException e)
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)e.Response;
                _logger.Error(e);
                return new Page { Url = url, Html = string.Empty, Timestamp = DateTime.UtcNow, ResponseCode = httpWebResponse.StatusCode };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new Page { Url = url, Html = string.Empty, Timestamp = DateTime.UtcNow };
            }
        }

        public static Page Download(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AllowAutoRedirect = true;
            webRequest.UserAgent = UserAgent.IE11.Get();
            try
            {
                using (var response = webRequest.GetResponse())
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)response;
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream(), true))
                    {
                        string html = streamReader.ReadToEnd();
                        return new Page { Url = url, Html = html, Timestamp = DateTime.Now, ResponseCode = httpWebResponse.StatusCode };
                    }
                }
            }
            catch (WebException e)
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)e.Response;
                _logger.Error(e);
                return new Page { Url = url, Html = string.Empty, Timestamp = DateTime.UtcNow, ResponseCode = httpWebResponse.StatusCode };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new Page { Url = url, Html = string.Empty, Timestamp = DateTime.UtcNow };
            }
        }
    }
}
