using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WePrint.Data;

namespace WePrint.ViewModels
{
    public class JobViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }


        public Guid CustomerId { get; set; }

        public Guid? MakerId { get; set; }

        public JobStatus Status { get; set; }

        public string Description { get; set; }

        public PrinterType? PrinterType { get; set; }

        public MaterialType? MaterialType { get; set; }

        public MaterialColor? MaterialColor { get; set; }

        public string? Notes { get; set; }

        public DateTimeOffset BidClose { get; set; }

        public virtual Address Address { get; set; }
    }
}
