using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Identity;

namespace WePrint.Data
{
    public class WePrintUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
