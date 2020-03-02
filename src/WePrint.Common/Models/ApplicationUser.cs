using System.Collections.Generic;
using Raven.Identity;

namespace WePrint.Common.Models
{
    public class ApplicationUser : IdentityUser, IDbModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<PrinterModel> PrinterIds { get; set; }
        public string Bio { get; set; }
        public List<string> ReviewIds { get; set; }
        public AddressModel Address { get; set; }
    }
}
