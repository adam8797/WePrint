using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Data
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Goal { get; set; }

        public string? ShippingInstructions { get; set; }

        public string? PrintingInstructions { get; set; }

        [MaxLength(2000)]
        [DataType(DataType.Url)]
        public string? Thumbnail { get; set; }

        [Required]
        public virtual Address Address { get; set; }

        [Required]
        public bool Closed { get; set; }

        [Required]
        public bool OpenGoal { get; set; }

        [Required]
        public virtual Organization Organization { get; set; }

        public virtual IList<Pledge> Pledges { get; set; }
        public virtual IList<ProjectUpdate> Updates { get; set; }
    }
}
