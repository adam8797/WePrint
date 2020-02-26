using System;
using System.Collections.Generic;
using System.Text;

namespace WePrint.Common
{
    public enum MachineType
    {
        FDM,
        SLA,
    }

    public enum JobStatus
    {
        Waiting,
        Processing,
        Error,
        Complete
    }
}
