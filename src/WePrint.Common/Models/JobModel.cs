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
        public JobStatus Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PrinterType PrinterType { get; set; }
        public MaterialType MaterialType { get; set; }
        public MaterialColor MaterialColor { get; set; }
        public string Notes { get; set; }
        public DateTime BidClose { get; set; }

        public ICollection<BidModel> Bids { get; set; } = new List<BidModel>();
        public ICollection<SliceReport> SliceReports { get; set; } = new List<SliceReport>();
        public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();

        public void ApplyChanges(JobUpdateModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
        }

        public JobModel GetViewableJob(string userId)
        {
            JobModel returnable = this;
            if (returnable.CustomerId != userId && returnable.Status < JobStatus.Closed)
                returnable.Bids = returnable.Bids.Where(b => b.BidderId == userId).ToList();
            if (returnable.MakerId != userId && returnable.CustomerId != userId)
            {
                returnable.Comments = new List<CommentModel>();
                returnable.Address = Address.GetPublicAddress();
            }

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

    public class JobViewModel
    {
        public string Id { get; set; }
        public BidViewModel AcceptedBid { get; set; }
        public string CustomerId { get; set; }
        public string CustomerUserName { get; set; }
        public string MakerId { get; set; }
        public string MakerUserName { get; set; }
        public AddressModel Address { get; set; }
        public ICollection<BidViewModel> Bids { get; set; }
        public ICollection<SliceReport> SliceReports { get; set; } = new List<SliceReport>();
        public JobStatus Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PrinterType PrinterType { get; set; }
        public MaterialType MaterialType { get; set; }
        public MaterialColor MaterialColor { get; set; }
        public string Notes { get; set; }
        public DateTime BidClose { get; set; }
        public ICollection<CommentViewModel> Comments { get; set; }

        public JobViewModel(JobModel job, IEnumerable<ApplicationUser> users, string userId)
        {
            JobModel viewableJob = job.GetViewableJob(userId); 
            Id = viewableJob.Id; // Reflection Helper doesn't work here for some reason, TODO figure that out and use it instead
            CustomerId = viewableJob.CustomerId;
            MakerId = viewableJob.MakerId;
            Address = viewableJob.Address;
            Status = viewableJob.Status;
            Description = viewableJob.Description;
            PrinterType = viewableJob.PrinterType;
            MaterialType = viewableJob.MaterialType;
            MaterialColor = viewableJob.MaterialColor;
            SliceReports = viewableJob.SliceReports;
            Notes = viewableJob.Notes;
            BidClose = viewableJob.BidClose;
            Name = viewableJob.Name;

            if (CustomerId != null)
                CustomerUserName = users.FirstOrDefault(user => user.Id == CustomerId).UserName;
            if (MakerId != null)
                MakerUserName = users.FirstOrDefault(user => user.Id == MakerId).UserName;
            if (viewableJob.AcceptedBid != null)
                AcceptedBid = new BidViewModel(viewableJob.AcceptedBid, users);
            if (viewableJob.Bids.Any())
                Bids = viewableJob.Bids.Select(bid => new BidViewModel(bid, users)).ToList();
            if (viewableJob.Comments.Any())
                Comments = viewableJob.Comments.Select(comment => new CommentViewModel(comment, users)).ToList();
        }
    }
}