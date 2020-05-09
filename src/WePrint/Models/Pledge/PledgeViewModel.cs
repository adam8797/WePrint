using System;

namespace WePrint.Models
{
    public class PledgeViewModel
    {
        public Guid Id { get; set; }

        public DateTimeOffset DeliveryDate { get; set; }

        public DateTimeOffset Created { get; set; }

        public int Quantity { get; set; }

        public PledgeStatus Status { get; set; }

        public bool Anonymous { get; set; }

        public Guid Project { get; set; }

        public UserViewModel Maker { get; set; }

        public bool Deleted { get; set; }
    }

    public class PledgeViewModelNoUser
    {
        public Guid Id { get; set; }

        public DateTimeOffset DeliveryDate { get; set; }

        public DateTimeOffset Created { get; set; }

        public int Quantity { get; set; }

        public PledgeStatus Status { get; set; }

        public bool Anonymous { get; set; }

        public Guid Project { get; set; }

        public Guid Maker { get; set; }

        public bool Deleted { get; set; }
    }
}
