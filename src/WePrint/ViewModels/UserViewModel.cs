using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.ViewModels
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
