﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WePrint.Models;

namespace WePrint.Data
{
    public class ProjectUpdate : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset? EditTimestamp { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Body { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public virtual User PostedBy { get; set; }

        [Required]
        public virtual Project Project { get; set; }

        public bool Deleted { get; set; }
    }
}
