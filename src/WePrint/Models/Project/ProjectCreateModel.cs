using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WePrint.Data;

namespace WePrint.Models.Project
{
    public class ProjectCreateModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public int Goal { get; set; }

        public string? ShippingInstructions { get; set; }

        public string? PrintingInstructions { get; set; }

        [MaxLength(2000)]
        [DataType(DataType.Url)]
        public string? Thumbnail { get; set; }

        public Address? Address { get; set; }

        [Required]
        public bool Closed { get; set; }

        [Required]
        public bool OpenGoal { get; set; }
    }
}
