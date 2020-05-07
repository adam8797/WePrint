using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WePrint.Models;

namespace WePrint.Data
{
    public class project_update : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset timestamp { get; set; }

        public DateTimeOffset edit_timestamp { get; set; }

        [Required]
        [MaxLength(4000)]
        public string body { get; set; }

        [Required]
        [MaxLength(100)]
        public string title { get; set; }

        [Required]
        public virtual user posted_by { get; set; }

        [Required]
        public virtual project project { get; set; }

        public bool Deleted { get; set; }
    }
}
