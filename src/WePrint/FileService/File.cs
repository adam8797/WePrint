using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WePrint.Data
{
    public class File
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(150)]
        public string FilePath { get; set; }

        [MaxLength(50)]
        public string MimeType { get; set; }

        [Column("varbinary(MAX)")]
        public byte[] Data { get; set; }
    }
}
