using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{
    public class project_create_model
    {
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

        public string? printing_instructions { get; set; }

        public Address? address { get; set; }

        [Required]
        public bool closed { get; set; }

        [Required]
        public bool open_goal { get; set; }
    }
}
