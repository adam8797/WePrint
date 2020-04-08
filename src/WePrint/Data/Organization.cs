using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Data
{

    public class Organization
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Url)]
        [MaxLength(2000)]
        public string? Logo { get; set; }

        [MaxLength(2000)]
        [Required]
        public string Description { get; set; }

        public virtual IList<User> Users { get; set; }

        public virtual IList<Project> Projects { get; set; }


    }
}
