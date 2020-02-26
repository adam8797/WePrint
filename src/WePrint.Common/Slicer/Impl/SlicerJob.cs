using System;
using System.Collections.Generic;
using System.Text;
using WePrint.Common.Slicer.Interface;

namespace WePrint.Common.Slicer.Impl
{
    public class SlicerJob
    {
        public string Id { get; set; }
        public string Job { get; set; }
        public ICollection<string> Files { get; set; }
        public string Worker { get; set; }
        public JobStatus Status { get; set; }
    }
}
