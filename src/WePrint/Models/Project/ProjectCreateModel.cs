using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{
    public class ProjectCreateModel
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Description { get; set; }

        [Required]
        public int Goal { get; set; }

        [MaxLength(4000)]
        public string? ShippingInstructions { get; set; }

        public string? PrintingInstructions { get; set; }

        public Address? Address { get; set; }

        [Required]
        public bool Closed { get; set; }

        [Required]
        public bool OpenGoal { get; set; }
    }
}
