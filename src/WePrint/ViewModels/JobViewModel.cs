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

        public string Customer { get; set; }
        public string Maker { get; set; }
        public Address Address { get; set; }
        public JobStatus Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PrinterType PrinterType { get; set; }
        public MaterialType MaterialType { get; set; }
        public MaterialColor MaterialColor { get; set; }
        public string Notes { get; set; }
        public DateTime BidClose { get; set; }

        public virtual IList<Bid> Bids { get; set; } = new List<Bid>();

        public virtual IList<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();

        public virtual IList<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();
    }
}
