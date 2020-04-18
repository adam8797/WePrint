using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{

    public class Organization : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Url)]
        [MaxLength(2000)]
        public string? Logo { get; set; }

        [MaxLength(4000)]
        [Required]
        public string Description { get; set; }

        public virtual IList<User> Users { get; set; } = new List<User>();

        public virtual IList<Project> Projects { get; set; } = new List<Project>();

        public virtual Address Address { get; set; }

        public bool Deleted { get; set; }
    }
}
