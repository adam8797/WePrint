using System.Collections.Generic;

namespace WePrint.Common.Slicer.Models
{
    public class SliceReport
    {
        public string FileName { get; set; }

        public Volume Volume { get; set; }

        public ICollection<TimeEstimate> TimeEstimates { get; set; }

        public ICollection<MaterialEstimate> MaterialEstimates { get; set; }
    }
}
