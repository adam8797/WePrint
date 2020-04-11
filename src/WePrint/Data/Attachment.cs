using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WePrint.Models.Job;
using WePrint.Models.Project;
using WePrint.Models.User;

namespace WePrint.Data
{
    public class JobAttachment
    {
        [Required]
        public virtual Job Owner { get; set; }

        [Required]
        public virtual User SubmittedBy { get; set; }

        [Required]
        [MaxLength(2000)]
        public string URL { get; set; }
    }

    public class ProjectAttachment
    {
        [Required]
        public virtual Project Owner { get; set; }

        [Required]
        public virtual User SubmittedBy { get; set; }

        [Required]
        [MaxLength(2000)]
        public string URL { get; set; }
    }
}
