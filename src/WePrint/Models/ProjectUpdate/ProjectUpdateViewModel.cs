using System;

namespace WePrint.Data
{
    public class ProjectUpdateViewModel
    {
        public Guid Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset EditTimestamp { get; set; }

        public string Body { get; set; }

        public string Title { get; set; }

        public Guid PostedBy { get; set; }

        public Guid Project { get; set; }

        public bool Deleted { get; set; }
    }
}