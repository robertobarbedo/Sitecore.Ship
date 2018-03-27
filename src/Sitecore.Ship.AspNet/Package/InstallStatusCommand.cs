using System;
using System.Linq;
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
    class InstallStatusCommand : CommandHandler
    {
        private readonly IJobTracker _jobTracker;

        public InstallStatusCommand(IJobTracker jobTracker)
        {
            _jobTracker = jobTracker;
        }

        public InstallStatusCommand()
            : this(new JobTracker())
        {
        }

        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                try
                {
                    var token = GetToken(context.Request);
                    var status = _jobTracker.JobStatus(token);

                    if (status != JobConstants.Running)
                    {
                        DisposePackageFile(token);
                    }

                    string json;

                    if (status != JobConstants.Error || _jobTracker.GetExceptions() == null)
                        json = Json.Encode(new { Status = status });
                    else
                        json = Json.Encode(new { Status = status, Message = String.Join("|", _jobTracker.GetExceptions().Select(c => c.Message).ToArray()) });

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
                   context.Request.Url.PathAndQuery.EndsWith("/services/package/install/installstatus", StringComparison.InvariantCultureIgnoreCase) &&
                   context.Request.HttpMethod == "POST";
        }

        private static String GetToken(HttpRequestBase request)
        {
            return request.Form["token"];
        }
    }
}
