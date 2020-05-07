using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{
    public class project : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string title { get; set; }

        [Required]
        [MaxLength(4000)]
        public string description { get; set; }

        [Required]
        public int goal { get; set; }

        [MaxLength(4000)]
        public string? shipping_instructions { get; set; }

        [MaxLength(4000)]
        public string? printing_instructions { get; set; }

        [Required]
        public virtual Address address { get; set; }

        [Required]
        public bool closed { get; set; }

        [Required]
        public bool open_goal { get; set; }

        [Required]
        public virtual organization organization { get; set; }

        public virtual IList<pledge> pledges { get; set; } = new List<pledge>();

        public virtual IList<project_update> updates { get; set; } = new List<project_update>();

        public bool Deleted { get; set; }
    }
}
