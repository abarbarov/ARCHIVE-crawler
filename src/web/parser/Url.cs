using System.Text;

namespace rift.parser
{
    public class Url
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string Parameters { get; set; }
        public string Query { get; set; }
        public string Fragment { get; set; }

        public Url()
        {
        }

        public Url(Url url)
        {
            Scheme = url.Scheme;
            Host = url.Host;
            Path = url.Path;
            Parameters = url.Parameters;
            Query = url.Query;
            Fragment = url.Fragment;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Scheme != null)
            {
                sb.Append(Scheme);
                sb.Append(':');
            }
            if (Host != null)
            {
                sb.Append("//");
                sb.Append(Host);
            }
            if (Path != null)
            {
                sb.Append(Path);
            }
            if (Parameters != null)
            {
                sb.Append(';');
                sb.Append(Parameters);
            }
            if (Query != null)
            {
                sb.Append('?');
                sb.Append(Query);
            }
            if (Fragment != null)
            {
                sb.Append('#');
                sb.Append(Fragment);
            }

            return sb.ToString();
        }
    }
}
