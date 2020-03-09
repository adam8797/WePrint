using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WePrint.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SliceJobStatus
    {
        Waiting,
        Processing,
        Error,
        Complete
    }
}
