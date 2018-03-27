using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Ship.Core.Domain
{
    public class JobConstants
    {
        public const string JobName = "Sitecore.Ship";
        public const string JobSiteName = "shell";
        public const string JobCategory = "Deployment";

        public const string NotFound = "NotFoundOrRestarted";
        public const string Running = "Running";
        public const string Success = "Success";
        public const string Queued = "Queued";
        public const string Error = "Error";
    }
}
