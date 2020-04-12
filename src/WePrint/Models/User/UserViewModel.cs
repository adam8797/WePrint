using System;
using System.ComponentModel.DataAnnotations;

namespace WePrint.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        [MaxLength(150)]
        public string? FirstName { get; set; }

        [MaxLength(150)]
        public string? LastName { get; set; }

        public string? Bio { get; set; }

        public string Username { get; set; }
    }
}
