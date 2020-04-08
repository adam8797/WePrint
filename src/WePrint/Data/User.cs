using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WePrint.Data
{
    public class User : IdentityUser<Guid>
    {
        [MaxLength(150)]
        public string? FirstName { get; set; }
        
        [MaxLength(150)]
        public string? LastName { get; set; }
        
        public string? Bio { get; set; }

        public virtual Organization? Organization { get; set; }

        public virtual IList<Printer> Printers { get; set; }
        public virtual IList<Review> Reviews { get; set; }
        public virtual IList<Bid> Bids { get; set; }
        public virtual IList<Job> Jobs { get; set; }
        public virtual IList<Pledge> Pledges { get; set; }
    }
}
