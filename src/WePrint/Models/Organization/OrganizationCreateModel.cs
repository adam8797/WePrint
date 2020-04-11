using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WePrint.Data;

namespace WePrint.Models.Organization
{
    public class OrganizationCreateModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Url)]
        [MaxLength(2000)]
        public string? Logo { get; set; }

        [MaxLength(2000)]
        [Required]
        public string Description { get; set; }
    }
}
