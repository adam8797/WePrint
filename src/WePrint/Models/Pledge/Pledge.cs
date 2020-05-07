using System;
using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{
    public class pledge : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset delivery_date { get; set; }

        [Required]
        public DateTimeOffset created { get; set; }

        [Required]
        public int quantity { get; set; }

        [Required]
        public PledgeStatus status { get; set; }

        [Required]
        public bool anonymous { get; set; }

        [Required]
        public virtual project project { get; set; }

        [Required]
        public virtual user maker { get; set; }

        public bool Deleted { get; set; }
    }
}
