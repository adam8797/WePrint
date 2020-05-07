using System;
using System.ComponentModel.DataAnnotations;

namespace WePrint.Models
{
    public class user_view_model
    {
        public Guid id { get; set; }

        [MaxLength(150)]
        public string? first_name { get; set; }

        [MaxLength(150)]
        public string? last_name { get; set; }

        public Guid? organization { get; set; }

        public string? bio { get; set; }

        public string username { get; set; }

        public bool deleted { get; set; }
    }
}
