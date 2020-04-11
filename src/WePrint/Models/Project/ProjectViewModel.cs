using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WePrint.Data;

namespace WePrint.Models.Project
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public int Goal { get; set; }

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
    }
}
