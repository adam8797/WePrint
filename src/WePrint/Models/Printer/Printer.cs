using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WePrint.Data;

namespace WePrint.Models
{
    public class Printer : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public virtual User Owner { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public PrinterType Type { get; set; }
        
        [Required]
        public int XMax { get; set; } //mm
        
        [Required]
        public int YMax { get; set; } //mm
        
        [Required]
        public int ZMax { get; set; }  //mm

        [Column(TypeName = "decimal(6,3)")]
        [Required]
        public decimal LayerMin { get; set; } //mm
    }
}