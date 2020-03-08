using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Models
{
    public class PasswordChangeModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
