using System.ComponentModel.DataAnnotations;

namespace WePrint.Data
{
    public class project_update_create_model
    {
        [Required]
        [MaxLength(4000)]
        public string body { get; set; }

        [Required]
        [MaxLength(100)]
        public string title { get; set; }
    }
}