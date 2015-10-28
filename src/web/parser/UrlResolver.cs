using System;
using System.Linq;

namespace rift.parser
{
    public class UrlResolver
    {
        public static string ResolveUrl(string baseUrl, string relativeUrl)
        {
            if (baseUrl == null)
            {
                throw new ArgumentNullException("Base URL must not be null");
            }
            if (relativeUrl == null)
            {
                throw new ArgumentNullException("Relative URL must not be null");
            }
            if (baseUrl == string.Empty && relativeUrl.StartsWith("//"))
            {
                relativeUrl = string.Concat("http:", relativeUrl);
            }
            Url url = ResolveUrl(ParseUrl(baseUrl.Trim()), relativeUrl.Trim());

            return url.ToString();
        }

        private static int IndexOf(string s, char searchChar, int beginIndex, int endIndex)
        {
            for (int i = beginIndex; i < endIndex; i++)
            {
                if (s.ElementAt(i) == searchChar)
                {
                    return i;
                }
            }
            return -1;
        }

        private static Url ParseUrl(string spec)
        {
            Url url = new Url();
            int startIndex = 0;
            int endIndex = spec.Length;

            int crosshatchIndex = IndexOf(spec, '#', startIndex, endIndex);

            if (crosshatchIndex >= 0)
            {
                url.Fragment = spec.Substring(crosshatchIndex + 1, endIndex - crosshatchIndex - 1);
                endIndex = crosshatchIndex;
            }
            
            int colonIndex = IndexOf(spec, ':', startIndex, endIndex);

            if (colonIndex > 0)
            {
                string scheme = spec.Substring(startIndex, colonIndex - startIndex);
                if (IsValidScheme(scheme))
                {
                    url.Scheme = scheme;
                    startIndex = colonIndex + 1;
                }
            }
            int locationStartIndex;
            int locationEndIndex;

            if (spec.IndexOf("//", startIndex) != -1)
            {
                locationStartIndex = startIndex + 2;
                locationEndIndex = IndexOf(spec, '/', locationStartIndex, endIndex);
                if (locationEndIndex >= 0)
                {
                    startIndex = locationEndIndex;
                }
            }
            else
            {
                locationStartIndex = -1;
                locationEndIndex = -1;
            }
            int questionMarkIndex = IndexOf(spec, '?', startIndex, endIndex);

            if (questionMarkIndex >= 0)
            {
                if ((locationStartIndex >= 0) && (locationEndIndex < 0))
                {
                    locationEndIndex = questionMarkIndex;
                    startIndex = questionMarkIndex;
                }
                url.Query = spec.Substring(questionMarkIndex + 1, endIndex - questionMarkIndex - 1);
                endIndex = questionMarkIndex;
            }
            int semicolonIndex = IndexOf(spec, ';', startIndex, endIndex);

            if (semicolonIndex >= 0)
            {
                if ((locationStartIndex >= 0) && (locationEndIndex < 0))
                {
                    locationEndIndex = semicolonIndex;
                    startIndex = semicolonIndex;
                }
                url.Parameters = spec.Substring(semicolonIndex + 1, endIndex - semicolonIndex - 1);
                endIndex = semicolonIndex;
            }
            if ((locationStartIndex >= 0) && (locationEndIndex < 0))
            {
                locationEndIndex = endIndex;
            }
            else if (startIndex < endIndex)
            {
                url.Path = spec.Substring(startIndex, endIndex - startIndex);
            }
            if ((locationStartIndex >= 0) && (locationEndIndex >= 0))
            {
                url.Host = spec.Substring(locationStartIndex, locationEndIndex - locationStartIndex);
            }
            return url;
        }

        private static bool IsValidScheme(string scheme)
        {
            int length = scheme.Length;
            if (length < 1)
            {
                return false;
            }
            char c = scheme.ElementAt(0);
            if (!Char.IsLetter(c))
            {
                return false;
            }
            for (int i = 1; i < length; i++)
            {
                c = scheme.ElementAt(i);
                if (!Char.IsLetterOrDigit(c) && c != '.' && c != '+' && c != '-')
                {
                    return false;
                }
            }
            return true;
        }

        private static Url ResolveUrl(Url baseUrl, string relativeUrl)
        {
            Url url = ParseUrl(relativeUrl);
            if (baseUrl == null)
            {
                return url;
            }
            if (relativeUrl.Length == 0)
            {
                return new Url(baseUrl);
            }
            if (url.Scheme != null)
            {
                return url;
            }
            url.Scheme = baseUrl.Scheme;
            if (url.Host != null)
            {
                return url;
            }
            url.Host = baseUrl.Host;
            if (!string.IsNullOrEmpty(url.Path) && '/' == url.Path.ElementAt(0))
            {
                url.Path = RemoveLeadingSlashPoints(url.Path);
                return url;
            }
            if (url.Path == null)
            {
                url.Path = baseUrl.Path;
                if (url.Parameters != null)
                {
                    return url;
                }
                url.Parameters = baseUrl.Parameters;
                if (url.Query != null)
                {
                    return url;
                }
                url.Query = baseUrl.Query;
                return url;
            }
            string basePath = baseUrl.Path;
            string path = "";

            if (basePath != null)
            {
                int lastSlashIndex = basePath.LastIndexOf('/');

                if (lastSlashIndex >= 0)
                {
                    path = basePath.Substring(0, lastSlashIndex + 1);
                }
            }
            else
            {
                path = "/";
            }
            path = string.Concat(path, url.Path);
            
            int pathSegmentIndex;

            while ((pathSegmentIndex = path.IndexOf("/./")) >= 0)
            {
                path = string.Concat(path.Substring(0, pathSegmentIndex + 1), path.Substring(pathSegmentIndex + 3));
            }
            if (path.EndsWith("/."))
            {
                path = path.Substring(0, path.Length - 1);
            }
            while ((pathSegmentIndex = path.IndexOf("/../")) > 0)
            {
                string pathSegment = path.Substring(0, pathSegmentIndex);
                int slashIndex = pathSegment.LastIndexOf('/');

                if (slashIndex < 0)
                {
                    continue;
                }
                if (!"..".Equals(pathSegment.Substring(slashIndex)))
                {
                    path = string.Concat(path.Substring(0, slashIndex + 1), path.Substring(pathSegmentIndex + 4));
                }
            }
            if (path.EndsWith("/.."))
            {
                string pathSegment = path.Substring(0, path.Length - 3);
                int slashIndex = pathSegment.LastIndexOf('/');

                if (slashIndex >= 0)
                {
                    path = path.Substring(0, slashIndex + 1);
                }
            }

            path = RemoveLeadingSlashPoints(path);

            url.Path = path;

            return url;
        }

        private static string RemoveLeadingSlashPoints(string path)
        {
            while (path.StartsWith("/.."))
            {
                path = path.Substring(3);
            }

            return path;
        }
    }
}