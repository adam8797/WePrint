using System;
using Newtonsoft.Json;
using WePrint.Common.Models;

namespace WePrint.Common.Slicer.Models
{
    public class TimeEstimate
    {
        public PrinterType PrinterType;

        [JsonConverter(typeof(TimespanConverter))]
        public TimeSpan TimeSpan;
    }
}
