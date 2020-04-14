using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Models
{
    public class SearchViewModel
    {
        public string Title { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public Guid Id { get; set; }

        public string Type { get; set; }
    }
}
