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

namespace Sitecore.Ship.AspNet.Package
{
    class InstallUploadPackageAsyncCommand : CommandHandler
    {
        private readonly IPackageRepository _repository;
        private readonly ITempPackager _tempPackager;
        private readonly IInstallationRecorder _installationRecorder;

        public InstallUploadPackageAsyncCommand(IPackageRepository repository, ITempPackager tempPackager, IInstallationRecorder installationRecorder)
        {
            _repository = repository;
            _tempPackager = tempPackager;
            _installationRecorder = installationRecorder;
        }

        public InstallUploadPackageAsyncCommand()
            : this(new PackageRepository(new UpdatePackageRunnerAsJob(new PackageManifestReader())),
                   new TempPackager(new ServerTempFile()),
                   new InstallationRecorder(new PackageHistoryRepository(), new PackageInstallationConfigurationProvider().Settings))
        {
        }


        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                try
                {
                    if (context.Request.Files.Count == 0)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }

                    var file = context.Request.Files[0];

                    var uploadPackage = GetRequest(context.Request);

                    var package = new InstallPackage { Path = _tempPackager.GetPackageToInstall(file.InputStream) };
                    
                    _repository.AddPackage(package);

                    _installationRecorder.RecordInstall(uploadPackage.PackageId, uploadPackage.Description, DateTime.Now);

                    string token = System.IO.Path.GetFileNameWithoutExtension(package.Path);

                    var json = Json.Encode(new { token });

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

        private static bool CanHandle(HttpContextBase context)
        {
            return context.Request.Url != null &&
                   context.Request.Url.PathAndQuery.EndsWith("/services/package/install/fileuploadasync", StringComparison.InvariantCultureIgnoreCase) &&
                   context.Request.HttpMethod == "POST";
        }

        private static InstallUploadPackage GetRequest(HttpRequestBase request)
        {
            return new InstallUploadPackage
            {
                PackageId = request.Form["packageId"],
                Description = request.Form["description"]
            };
        }
    }
}
