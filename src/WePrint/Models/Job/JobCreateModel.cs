using System;
using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{
    public class JobCreateModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public PrinterType? PrinterType { get; set; }

        public MaterialType? MaterialType { get; set; }

        public MaterialColor? MaterialColor { get; set; }

        public string? Notes { get; set; }

        public DateTimeOffset BidClose { get; set; }

        public virtual Address Address { get; set; }
    }
}