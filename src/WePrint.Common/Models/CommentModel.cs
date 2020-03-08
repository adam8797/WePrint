using System;
using System.Collections.Generic;
using System.Linq;

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

    public class CommentViewModel
    {
        public int Id { get; set; }

        public string CommenterId { get; set; }

        public string CommenterUserName { get; set; }

        public string Text { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Edited { get; set; }

        public CommentViewModel(CommentModel fromComment, IEnumerable<ApplicationUser> users)
        {
            ReflectionHelper.CopyPropertiesTo(fromComment, this);
            CommenterUserName = users.FirstOrDefault(user => user.Id == fromComment.CommenterId).UserName;
        }
    }
}