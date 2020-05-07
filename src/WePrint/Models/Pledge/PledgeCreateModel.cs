using System;
using System.ComponentModel.DataAnnotations;

namespace WePrint.Models
{
    public class pledge_create_model
    {
        [Required]
        public DateTimeOffset delivery_date { get; set; }

        [Required]
        public int quantity { get; set; }

        [Required]
        public bool anonymous { get; set; }
    }
}
