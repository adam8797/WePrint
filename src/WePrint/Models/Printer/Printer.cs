using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WePrint.Data;

namespace WePrint.Models
{
    public class printer : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public virtual user owner { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [Required]
        public PrinterType type { get; set; }
        
        [Required]
        public int x_max { get; set; } //mm
        
        [Required]
        public int y_max { get; set; } //mm
        
        [Required]
        public int z_max { get; set; }  //mm

        [Column(TypeName = "decimal(6,3)")]
        [Required]
        public decimal layer_min { get; set; } //mm

        public bool Deleted { get; set; }
    }
}