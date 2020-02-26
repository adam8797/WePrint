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

        public void ApplyChanges(ApplicationUserUpdateModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
        }
    }

    public class ApplicationUserUpdateModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public AddressModel Address { get; set; }
    }
}
