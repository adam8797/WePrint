using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WePrint.Data
{
    [Owned]
    public class Address
    {
        [MaxLength(150)]
        [Required]
        public string Attention { get; set; }

        [MaxLength(150)]
        [Required]
        public string? AddressLine1 { get; set; }

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

        public override string ToString()
        {
            var lines = new[] {Attention, AddressLine1, AddressLine2, AddressLine3, $"{City}, {State} {ZipCode}"};
            return string.Join('\n', lines.Where(x => !string.IsNullOrEmpty(x)));
        }

        public static implicit operator string?(Address? a)
        {
            return a?.ToString();
        }
    }
}