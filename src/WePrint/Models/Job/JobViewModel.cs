using System;
using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models.Job
{
    public class JobViewModel : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

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

        public Address Address { get; set; }
    }
}
