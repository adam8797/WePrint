using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace WePrint.Data
{
    public class Bid
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "smallmoney")]
        public decimal Price { get; set; }

        [Required]
        public virtual User Bidder { get; set; }
        
        [Required]
        public virtual Job Job { get; set; }

        [Required]
        [Column("WorkTime")]
        public long WorkTimeTicks { get; set; }

        [NotMapped]
        public TimeSpan WorkTime
        {
            get => TimeSpan.FromTicks(WorkTimeTicks);
            set => WorkTimeTicks = value.Ticks;
        }

        [MaxLength(2000)]
        public string? Notes { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(6,3)")]
        public decimal LayerHeight { get; set; }


        [Required]
        [Column(TypeName = "decimal(6,3)")]
        public decimal ShellThickness { get; set; }

        [Required]
        [Column(TypeName = "decimal(6,3)")]
        public decimal FillPercentage { get; set; }
        
        [Required]
        public MaterialType MaterialType { get; set; }
        
        [Required]
        public MaterialColor MaterialColor { get; set; }
        
        [Required]
        public FinishType Finishing { get; set; }
        
        [Required]
        public bool Accepted { get; set; }
    }
}