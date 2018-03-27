using System.Web;
using Sitecore.Ship.AspNet.Package;
using Sitecore.Ship.AspNet.Publish;

namespace Sitecore.Ship.AspNet
{
    public class SitecoreShipHttpHandler : BaseHttpHandler
    {
        private readonly CommandHandler _commandChain;

        public SitecoreShipHttpHandler()
        {
            // TODO move this construction logic out of here ...

            var aboutCommand = new AboutCommand();

            var isJobRunningCommand = new IsJobRunningCommand();

            var installStatusCommand = new InstallStatusCommand();

            var installPackageCommand = new InstallPackageCommand();

            var installUploadPackageCommand = new InstallUploadPackageCommand();

            var installUploadPackageAsyncCommand = new InstallUploadPackageAsyncCommand();

            var latestVersionCommand = new LatestVersionCommand();

            var invokePublishingCommand = new InvokePublishingCommand();

            var publishingLastCompletedCommand = new PublishingLastCompletedCommand();

            var unhandledCommand = new UnhandledCommand();

            aboutCommand.SetSuccessor(isJobRunningCommand);

            isJobRunningCommand.SetSuccessor(installStatusCommand);

            installStatusCommand.SetSuccessor(installPackageCommand);

            installPackageCommand.SetSuccessor(installUploadPackageCommand);

            installUploadPackageCommand.SetSuccessor(installUploadPackageAsyncCommand);

            installUploadPackageAsyncCommand.SetSuccessor(latestVersionCommand);

            latestVersionCommand.SetSuccessor(invokePublishingCommand);

            invokePublishingCommand.SetSuccessor(publishingLastCompletedCommand);

            publishingLastCompletedCommand.SetSuccessor(unhandledCommand);

            _commandChain = aboutCommand;
        }

        public override void ProcessRequest(HttpContextBase context)
        {
            _commandChain.HandleRequest(context);
        }
    }
}