using System.ComponentModel.DataAnnotations;

namespace WePrint.Data
{
    public class ProjectUpdateCreateModel
    {
        [Required]
        [MaxLength(4000)]
        public string Body { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
    }
}