using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WePrint.Data
{
    public class Job
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public JobStatus Status { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public PrinterType? PrinterType { get; set; }
        
        public MaterialType? MaterialType { get; set; }
        
        public MaterialColor? MaterialColor { get; set; }

        public string? Notes { get; set; }

        [Required]
        public virtual User Customer { get; set; }

        [Required]
        public DateTimeOffset BidClose { get; set; }

        public virtual Address Address { get; set; }

        public virtual Bid? AcceptedBid { get; set; }
        
        public virtual IList<Bid> Bids { get; set; }
    }
}