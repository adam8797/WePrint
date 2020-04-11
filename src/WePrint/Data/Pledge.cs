using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WePrint.Models.Project;
using WePrint.Models.User;

namespace WePrint.Data
{
    public class Pledge : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column("DeliveryEstimate")]
        public long DeliveryEstimateTicks { get; set; }

        [NotMapped]
        public TimeSpan DeliveryEstimate
        {
            get => TimeSpan.FromTicks(DeliveryEstimateTicks);
            set => DeliveryEstimateTicks = value.Ticks;
        }

        [Required]
        public DateTimeOffset Created { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public PledgeStatus Status { get; set; }

        [Required]
        public bool Anonymous { get; set; }

        [Required]
        public virtual Project Project { get; set; }

        [Required]
        public virtual User Maker { get; set; }
    }
}
