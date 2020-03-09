using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Models
{
    public class SliceRequest
    {
        public string JobId { get; set; }
        public string[] Files { get; set; }
    }
}
