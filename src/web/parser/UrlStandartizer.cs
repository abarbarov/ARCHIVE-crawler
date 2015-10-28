using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace rift.parser
{
    public static class UrlStandartizer
    {
        public static string Standartize(this string href)
        {
            return Standartize(href, null);
        }

        public static string Standartize(this string href, string context)
        {
            Uri canonical;
            if (TryResolve(href, context, out canonical))
            {
                string path = canonical.LocalPath;

                // Convert '//' -> '/'
                int idx = path.IndexOf("//");
                while (idx >= 0)
                {
                    path = path.Replace("//", "/");
                    idx = path.IndexOf("//");
                }

                // Drop starting '/../'
                while (path.StartsWith("/../"))
                {
                    path = path.Substring(3);
                }

                // Trim
                path = path.Trim();

                SortedDictionary<string, string> parameters = CreateParameterMap(canonical.Query.Replace("?", ""));
                string queryString = string.Empty;

                if (parameters.Any())
                {
                    string canonicalParams = Canonicalize(parameters);
                    queryString = (string.IsNullOrEmpty(canonicalParams) ? "" : "?" + canonicalParams);
                }

                // Add starting slash if needed
                if (path.Length == 0)
                {
                    path = "/" + path;
                }

                string pathAndQuery = NormalizePath(path) + queryString;
                return Compose(canonical, pathAndQuery);
            }

            return string.Empty;
        }

        private static string Compose(Uri canonical, string pathAndQuery)
        {
            var url = new StringBuilder();
            url.Append(canonical.Scheme.ToLowerInvariant());
            url.Append(Uri.SchemeDelimiter);
            url.Append(canonical.Host.ToLowerInvariant());

            // Drop default port: example.com:80 -> example.com
            if (canonical.Port != 80)
            {
                url.Append(":");
                url.Append(canonical.Port);
            }
            url.Append(pathAndQuery);

            return url.ToString();
        }

        private static bool TryResolve(string href, string context, out Uri uri)
        {
            try
            {
                uri = new Uri(UrlResolver.ResolveUrl(context ?? string.Empty, href));

                if (string.IsNullOrEmpty(uri.Host))
                {
                    return false;
                }

                return true;
            }
            catch (UriFormatException)
            {
                uri = null;
                return false;
            }
            catch (Exception)
            {
                uri = null;
                return false;
            }
        }

        private static SortedDictionary<string, string> CreateParameterMap(string queryString)
        {
            var parameters = new SortedDictionary<string, string>();

            if (string.IsNullOrEmpty(queryString))
            {
                return parameters;
            }

            string[] pairs = queryString.Split('&');

            foreach (string pair in pairs)
            {
                if (pair.Length == 0)
                {
                    continue;
                }

                string[] tokens = pair.Split(new[] { '=' }, 2);
                switch (tokens.Length)
                {
                    case 1:
                        if (pair.ElementAt(0) == '=')
                        {
                            if (!parameters.ContainsKey(""))
                            {
                                parameters.Add("", tokens[0]);
                            }
                        }
                        else
                        {
                            if (!parameters.ContainsKey(tokens[0]))
                            {
                                parameters.Add(tokens[0], "");
                            }
                        }
                        break;
                    case 2:
                        if (!parameters.ContainsKey(tokens[0]))
                        {
                            parameters.Add(tokens[0], tokens[1]);
                        }
                        break;
                }
            }
            return parameters;
        }

        private static string Canonicalize(SortedDictionary<string, string> sortedParamMap)
        {
            if (!sortedParamMap.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder(100);
            foreach (var pair in sortedParamMap)
            {
                string key = pair.Key.ToLowerInvariant();
                if (key.Equals("jsessionid") || key.Equals("phpsessid") || key.Equals("aspsessionid"))
                {
                    continue;
                }
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(pair.Key.PercentEncodeRfc3986());
                if (!string.IsNullOrEmpty(pair.Value))
                {
                    sb.Append('=');
                    sb.Append(pair.Value.PercentEncodeRfc3986());
                }
            }
            return sb.ToString();
        }

        private static string PercentEncodeRfc3986(this string str)
        {
            try
            {
                str = str.Replace("+", "%2B");
                str = HttpUtility.UrlDecode(str, Encoding.UTF8);
                str = HttpUtility.UrlEncode(str, Encoding.UTF8);
                return str.Replace("+", "%20").Replace("*", "%2a").Replace("%7e", "~");
            }
            catch (Exception)
            {
                return str;
            }
        }

        private static string NormalizePath(string path)
        {
            return path.Replace("%7e", "~").Replace(" ", "%20");
        }
    }
}
