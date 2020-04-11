using System;
using System.ComponentModel.DataAnnotations;

namespace WePrint.Models.User
{
    public class UserViewModel
    {
        [MaxLength(150)]
        public string? FirstName { get; set; }

        [MaxLength(150)]
        public string? LastName { get; set; }

        public string? Bio { get; set; }

        public string Username { get; set; }

        public Guid Id { get; set; }
    }
}
