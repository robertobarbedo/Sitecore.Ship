using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Ship.Infrastructure
{
    public class JobTracker : IJobTracker
    {
        private List<Exception> exception { get; set; }

        public List<Exception> GetExceptions()
        {
            return exception;
        }

        public bool IsAnyRunning()
        {
            string jobName = String.Format(JobConstants.JobName, "");

            var count = Sitecore.Jobs.JobManager.GetJobs()
                .Where
                (c => c.Name.Equals(jobName, StringComparison.InvariantCultureIgnoreCase)
                      && !c.IsDone
                ).Count();
            return count != 0;
        }

        public string JobStatus(string token)
        {
            var job = GetShipJob(token);

            if (job == null)
            {
                return JobConstants.NotFound;
            }
            else if (!job.IsDone && job.Status.State != Jobs.JobState.Queued)
            {
                return JobConstants.Running;
            }
            else if (job.IsDone && job.Status.Failed)
            {
                this.exception = job.Status.Exceptions;
                return JobConstants.Error;
            }
            else if (job.Status.State == Jobs.JobState.Queued)
            {
                return JobConstants.Queued;
            }
            else
            {
                return JobConstants.Success;
            }
        }

        public static Jobs.Job GetShipJob(string token)
        {
            string jobName = String.Format(JobConstants.JobName, "");

            return Sitecore.Jobs.JobManager.GetJobs()
                .Where(c => c.Name.Equals(jobName, StringComparison.InvariantCultureIgnoreCase)
                            && (c.Options?.CustomData == null ? "" : c.Options.CustomData.ToString()).Equals(token, StringComparison.InvariantCultureIgnoreCase)
                ).FirstOrDefault();

        }
    }
}
