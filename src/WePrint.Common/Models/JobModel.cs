using System;
using System.Linq;
using System.Collections.Generic;
using WePrint.Common.Slicer.Models;

namespace WePrint.Common.Models
{
    public class JobModel : DbModel
    {
        public BidModel AcceptedBid { get; set; }
        public string CustomerId { get; set; }
        public string MakerId { get; set; }
        public AddressModel Address { get; set; }
        public ICollection<BidModel> Bids { get; set; }
        public ICollection<SliceReport> SliceReports { get; set; } = new List<SliceReport>();
        public JobStatus Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PrinterType PrinterType { get; set; }
        public MaterialType MaterialType { get; set; }
        public MaterialColor MaterialColor { get; set; }
        public string Notes { get; set; }
        public DateTime BidClose { get; set; }
        public ICollection<CommentModel> Comments { get; set; }

        public void ApplyChanges(JobUpdateModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
        }

        public JobModel GetViewableJob(string userId)
        {
            JobModel returnable = this;
            if (returnable.CustomerId != userId && returnable.Status < JobStatus.Closed)
            {
                returnable.Bids = returnable.Bids.Where(b => b.BidderId == userId).ToList();
            }
            if (returnable.MakerId != userId && returnable.CustomerId != userId)
                returnable.Comments = new List<CommentModel>();
                
            return returnable;
        }
    }

    public class JobUpdateModel
    {
        public string Id { get; set; }

        // Everything past here is optional
        public AddressModel Address { get; set; }
        public JobStatus? Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PrinterType? PrinterType { get; set; }
        public MaterialType? MaterialType { get; set; }
        public MaterialColor? MaterialColor { get; set; }
        public string Notes { get; set; }
        public DateTime? BidClose { get; set; }
    }
}