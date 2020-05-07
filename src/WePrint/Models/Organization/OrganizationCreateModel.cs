using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{
    public class organization_create_model
    {
        [Required]
        [MaxLength(100)]
        public string name { get; set; }

        [MaxLength(2000)]
        [Required]
        public string description { get; set; }

        public Address? address { get; set; }
    }
}
