using System;
using System.Net;
using System.Web;
using System.Web.Helpers;

using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Core.Services;
using Sitecore.Ship.Infrastructure.Configuration;
using Sitecore.Ship.Infrastructure.DataAccess;
using Sitecore.Ship.Infrastructure.IO;
using Sitecore.Ship.Infrastructure.Install;
using Sitecore.Ship.Infrastructure.Update;
using Sitecore.Ship.Infrastructure.Web;
using Sitecore.Ship.Infrastructure;
using System.IO;

namespace Sitecore.Ship.AspNet.Package
{
    class IsJobRunningCommand : CommandHandler
    {
        private readonly IJobTracker _jobTracker;

        public IsJobRunningCommand(IJobTracker jobTracker)
        {
            _jobTracker = jobTracker;
        }

        public IsJobRunningCommand()
            : this(new JobTracker())
        {
        }

        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                try
                {
                    var status = _jobTracker.IsAnyRunning();
                    
                    var json = Json.Encode(status);

                    JsonResponse(json, HttpStatusCode.Created, context);

                    context.Response.AddHeader("Location", ShipServiceUrl.PackageLatestVersion);
                }
                catch (NotFoundException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }

        private static void DisposePackageFile(string token)
        {
            //dispose package
            TempPackager disposable = new TempPackager(new ServerTempFileFromToken(token));
            disposable.GetPackageToInstall(new MemoryStream());//dummy cal as TempPackager need to fill internal name
            disposable.Dispose();
        }

        private static bool CanHandle(HttpContextBase context)
        {
            return context.Request.Url != null &&
                   context.Request.Url.PathAndQuery.EndsWith("/services/package/install/isjobrunning", StringComparison.InvariantCultureIgnoreCase) &&
                   context.Request.HttpMethod == "GET";
        }
    }
}
