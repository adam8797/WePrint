using System;
using System.Collections.Generic;
using System.Text;
using WePrint.Common.Slicer.Impl;
using WePrint.Common.Slicer.Interface;
using WePrint.Common.Slicer.Models;

namespace WePrint.Common.Models
{
    public class Job
    {
        public string Id { get; set; }

        public ICollection<SliceReport> SliceReports { get; set; } = new List<SliceReport>();
    }
}
