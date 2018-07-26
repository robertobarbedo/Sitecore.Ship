using System.Web;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Infrastructure.Web
{
    public class HttpRequestChecker : ICheckRequests
    {
        public bool IsLocal
        {
            get { return HttpContext.Current.Request.IsLocal; }
        }

        public string UserHostAddress
        {
            get
            {
                var header = Sitecore.Configuration.Settings.GetSetting("SitecoreShip.AuthenticateWith.HTTPHeader", "");
                if (string.IsNullOrWhiteSpace(header))
                    return ParseIP(HttpContext.Current.Request.UserHostAddress);
                else
                    return ParseIP(HttpContext.Current.Request.Headers[header] ?? HttpContext.Current.Request.ServerVariables[header]);
            }
        }

        protected virtual string ParseIP(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return ip;
            else if (ip.IndexOf(":") < 0)
                return ip;
            else
                return ip.Split(':')[0];
        }
    }
}