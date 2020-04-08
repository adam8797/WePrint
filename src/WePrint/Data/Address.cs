using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WePrint.Data
{
    [Owned]
    public class Address
    {
        [MaxLength(150)]
        [Required]
        public string AddressLine1 { get; set; }

        [MaxLength(150)]
        public string? AddressLine2 { get; set; }

        [MaxLength(150)]
        public string? AddressLine3 { get; set; }

        [MaxLength(50)]
        [Required]
        public string City { get; set; }

        [MaxLength(2)]
        [Column(TypeName = "nchar(2)")]
        [Required]
        public string State { get; set; }

        [Required]
        [Column(TypeName = "nchar(5)")]
        [MaxLength(5)]
        public string ZipCode { get; set; }
    }
}