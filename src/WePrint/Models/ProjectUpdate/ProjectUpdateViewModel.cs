using System;

namespace WePrint.Data
{
    public class project_update_view_model
    {
        public Guid id { get; set; }

        public DateTimeOffset timestamp { get; set; }

        public DateTimeOffset edit_timestamp { get; set; }

        public string body { get; set; }

        public string title { get; set; }

        public Guid posted_by { get; set; }

        public Guid project { get; set; }

        public bool deleted { get; set; }
    }
}