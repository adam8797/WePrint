using System;
using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{
    public class Pledge : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset DeliveryDate { get; set; }

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
