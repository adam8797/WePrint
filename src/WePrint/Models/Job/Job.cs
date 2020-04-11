using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models.Job
{
    public class Job : IIdentifiable<Guid>
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
        public virtual User.User Customer { get; set; }

        [Required]
        public DateTimeOffset BidClose { get; set; }

        public virtual Address Address { get; set; }

        public virtual Bid? AcceptedBid { get; set; }
        
        public virtual IList<Bid> Bids { get; set; } = new List<Bid>();

        public virtual IList<JobAttachment> Attachments { get; set; } = new List<JobAttachment>();
    }
}