using System;
using System.Collections.Generic;
using WePrint.Data;

namespace WePrint.Models
{
    public class organization_view_model
    {
        public Guid id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public List<Guid> users { get; set; }

        public List<Guid> projects { get; set; }

        public Address? address { get; set; }

        public bool deleted { get; set; }

        public bool should_serialize_deleted() => deleted;
    }


}
