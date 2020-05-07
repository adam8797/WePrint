using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Models
{
    public class search_view_model
    {
        public string title { get; set; }

        public string? description { get; set; }

        public string? image_url { get; set; }

        public string href { get; set; }

        public Guid id { get; set; }

        public string type { get; set; }
    }
}
