using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using WePrint.Data;

namespace WePrint.Models.User
{
    public class User : IdentityUser<Guid>
    {
        [MaxLength(150)]
        public string? FirstName { get; set; }
        
        [MaxLength(150)]
        public string? LastName { get; set; }
        
        public string? Bio { get; set; }

        public virtual Organization.Organization? Organization { get; set; }

        public virtual IList<Printer.Printer> Printers { get; set; } = new List<Printer.Printer>();
        public virtual IList<Review> Reviews { get; set; } = new List<Review>();
        public virtual IList<Bid> Bids { get; set; } = new List<Bid>();
        public virtual IList<Job.Job> Jobs { get; set; } = new List<Job.Job>();
        public virtual IList<Pledge> Pledges { get; set; } = new List<Pledge>();
    }
}
