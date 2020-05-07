using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{

    public class organization : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string name { get; set; }

        [MaxLength(4000)]
        [Required]
        public string description { get; set; }

        public virtual IList<user> users { get; set; } = new List<user>();

        public virtual IList<project> projects { get; set; } = new List<project>();

        public virtual Address address { get; set; }

        public bool Deleted { get; set; }
    }
}
