using System;
using System.Collections.Generic;
using WePrint.Data;
using System.Linq;

namespace WePrint.Models
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Goal { get; set; }

        public IDictionary<PledgeStatus, int> Progress { get; set; }

        public string? ShippingInstructions { get; set; }

        public string? PrintingInstructions { get; set; }

        public string? Thumbnail { get; set; }

        public Address Address { get; set; }

        public bool Closed { get; set; }

        public bool OpenGoal { get; set; }

        public Guid Organization { get; set; }

        public IList<Guid> Pledges { get; set; }

        public IList<Guid> Updates { get; set; }

        public IList<string> Attachments { get; set; }

        public bool Deleted { get; set; }
    }
}
