using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WePrint.Data
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public virtual User User { get; set; }

        public virtual Comment? Parent { get; set; }

        [Required]
        [MaxLength(512)]
        public string Body { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [Required]
        public bool Edited { get; set; }
    }
}