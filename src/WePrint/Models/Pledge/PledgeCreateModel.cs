using System;
using System.ComponentModel.DataAnnotations;

namespace WePrint.Models
{
    public class PledgeCreateModel
    {
        [Required]
        public DateTimeOffset DeliveryDate { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public bool Anonymous { get; set; }
    }
}
