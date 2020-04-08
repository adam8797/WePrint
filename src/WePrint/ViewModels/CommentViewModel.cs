using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WePrint.Data;

namespace WePrint.ViewModels
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }

        public Guid Job { get; set; }

        public string Commenter { get; set; }

        public string Text { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Edited { get; set; }
    }
}
