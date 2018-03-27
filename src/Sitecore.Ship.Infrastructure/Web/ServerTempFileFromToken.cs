using System;
using System.Web;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Infrastructure.Web
{
    public class ServerTempFileFromToken : ITempFile
    {
        private string _token;
        public ServerTempFileFromToken(string token)
        {
            _token = token;
        }
        public string Filename
        {
            get
            {
                string aux = Guid.NewGuid().ToString("N");

                string filename = Sitecore.IO.TempFolder.GetFilename(aux + ".update");

                //check if is not a physical path
                if (filename.StartsWith("/") || !System.IO.Path.IsPathRooted(filename))
                    filename = HttpContext.Current.Server.MapPath(filename);

                return filename.Replace(aux, _token);
            }
        }
    }
}