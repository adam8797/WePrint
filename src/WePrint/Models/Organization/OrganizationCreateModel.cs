﻿using System.ComponentModel.DataAnnotations;
using WePrint.Data;

namespace WePrint.Models
{
    public class OrganizationCreateModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(2000)]
        [Required]
        public string Description { get; set; }

        public Address? Address { get; set; }
    }
}
