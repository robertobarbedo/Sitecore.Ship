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
                    return HttpContext.Current.Request.UserHostAddress;
                else
                    return HttpContext.Current.Request.ServerVariables[header];
            }
        }
    }
}