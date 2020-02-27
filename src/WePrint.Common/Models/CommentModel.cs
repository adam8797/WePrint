using System;

namespace WePrint.Common.Models
{
    public class CommentModel
    {
        public int Id { get; set; }

        public string CommenterId { get; set; }

        public string Text { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Edited { get; set; }
    }
}