﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using WePrint.Data;

namespace WePrint.Models
{
    public class User : IdentityUser<Guid>, IIdentifiable<Guid>
    {
        [MaxLength(150)]
        public string? FirstName { get; set; }
        
        [MaxLength(150)]
        public string? LastName { get; set; }
        
        public string? Bio { get; set; }

        public virtual Organization? Organization { get; set; }

        public virtual IList<Printer> Printers { get; set; } = new List<Printer>();
        public virtual IList<Review> Reviews { get; set; } = new List<Review>();
        public virtual IList<Bid> Bids { get; set; } = new List<Bid>();
        public virtual IList<Job> Jobs { get; set; } = new List<Job>();
        public virtual IList<Pledge> Pledges { get; set; } = new List<Pledge>();
    }
}