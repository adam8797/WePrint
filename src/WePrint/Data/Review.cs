using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WePrint.Models;

namespace WePrint.Data
{
    public class Review : IIdentifiable<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Rating out of 10. Each level is a half star
        /// </summary>
        [Required]
        public int Rating { get; set; }
        
        [Required]
        public virtual Job Job { get; set; }
        
        [Required]
        [MaxLength(512)]
        public string Comment { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; }
        
        [Required]
        public virtual User Reviewer { get; set; }

        [Required]
        public virtual User ReviewedUser { get; set; }
    }
}