using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace WePrint.Models
{
    public class CommentModel : DbModel
    {
        public string CommenterId { get; set; }
        public string JobId { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }

        public void ApplyChanges(CommentUpdateModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
            Time = DateTime.Now;
        }
    }

    public class CommentUpdateModel
    {
        public string JobId {get; set;}
        public string Id { get; set; }
        public string Text { get; set; }
    }
}