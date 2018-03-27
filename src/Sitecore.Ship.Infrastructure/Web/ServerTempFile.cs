using System;
using System.Web;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Infrastructure.Web
{
    public class ServerTempFile : ITempFile
    {
        public string Filename
        {
            get
            {
                string name = Sitecore.IO.TempFolder.GetFilename(Guid.NewGuid() + ".update");

                if (name.StartsWith("/") || !System.IO.Path.IsPathRooted(name))
                    name = HttpContext.Current.Server.MapPath(name);

                return name;
            }
        }
    }
}