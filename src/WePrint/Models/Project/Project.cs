using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{
    public class Project : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Description { get; set; }

        [Required]
        public int Goal { get; set; }

        [MaxLength(4000)]
        public string? ShippingInstructions { get; set; }

        [MaxLength(4000)]
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

        public virtual IList<Pledge> Pledges { get; set; } = new List<Pledge>();

        public virtual IList<ProjectUpdate> Updates { get; set; } = new List<ProjectUpdate>();
    }
}
