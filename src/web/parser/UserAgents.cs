using System.ComponentModel;

namespace rift.parser
{
    public enum UserAgent
    {
        Default,
        IE8,
        IE9,
        IE10,
        [Description("Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko")]
        IE11,
        Chrome,
        Firefox,
        Safari
    }

    public static class UserAgents
    {
        public static string Get(this UserAgent value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}

