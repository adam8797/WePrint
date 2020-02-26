using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace WePrint.Models
{
    public class ReviewModel : DbModel
    {
        public int Rating { get; set; }
        public string JobId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string ReviewerId { get; set; }

        public void ApplyChanges(ReviewUpdateModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
            Date = DateTime.Now;
        }
    }

    public class ReviewUpdateModel
    {
        public string Id { get; set; }
        public int Rating { get; set; }
        public string JobId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string ReviewerId { get; set; }
    }
}