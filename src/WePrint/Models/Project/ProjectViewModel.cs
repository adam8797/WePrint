using System;
using System.Collections.Generic;
using WePrint.Data;
using System.Linq;

namespace WePrint.Models
{
    public class project_view_model
    {
        public Guid id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public int goal { get; set; }

        public IDictionary<PledgeStatus, int> progress { get; set; }

        public string? shipping_instructions { get; set; }

        public string? printing_instructions { get; set; }

        public Address address { get; set; }

        public bool closed { get; set; }

        public bool open_goal { get; set; }

        public Guid organization { get; set; }

        public IList<Guid> pledges { get; set; }

        public IList<Guid> updates { get; set; }

        public IList<string> attachments { get; set; }

        public bool deleted { get; set; }
    }
}
