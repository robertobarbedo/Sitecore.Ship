using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IJobTracker
    {
        string JobStatus(string token);

        List<Exception> GetExceptions();

        bool IsAnyRunning();
    }
}
