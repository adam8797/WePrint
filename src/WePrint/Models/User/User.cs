using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using WePrint.Data;

namespace WePrint.Models
{
    public class user : IdentityUser<Guid>, IIdentifiable<Guid>
    {
        [MaxLength(150)]
        public string? first_name { get; set; }
        
        [MaxLength(150)]
        public string? last_name { get; set; }
        
        public string? bio { get; set; }

        public virtual organization? organization { get; set; }

        public virtual IList<printer> printers { get; set; } = new List<printer>();

        public virtual IList<pledge> pledges { get; set; } = new List<pledge>();

        public bool Deleted { get; set; }
    }
}
