using System;

namespace WePrint.Models
{
    public class pledge_view_model
    {
        public Guid id { get; set; }

        public DateTimeOffset delivery_date { get; set; }

        public DateTimeOffset created { get; set; }

        public int quantity { get; set; }

        public PledgeStatus status { get; set; }

        public bool anonymous { get; set; }

        public Guid project { get; set; }

        public Guid maker { get; set; }

        public bool deleted { get; set; }
    }
}
