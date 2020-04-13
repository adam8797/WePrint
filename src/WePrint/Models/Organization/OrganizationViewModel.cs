using System;
using System.Collections.Generic;
using WePrint.Data;

namespace WePrint.Models
{
    public class OrganizationViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Logo { get; set; }

        public string Description { get; set; }

        public List<Guid> Users { get; set; }

        public List<Guid> Projects { get; set; }

        public Address? Address { get; set; }
    }
}
