using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Identity;
using System.Reflection;

namespace WePrint.Models
{
    public class ApplicationUser : IdentityUser, IDbModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<PrinterModel> Printers { get; set; }
        public string Bio { get; set; }
        public List<string> ReviewIds { get; set; }
        public AddressModel Address { get; set; }

        public void ApplyChanges(ApplicationUserUpdateModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
        }

        public ApplicationUser GetPublicUser()
        {
            ApplicationUser publicUser = new ApplicationUser
            {
                FirstName = FirstName,
                LastName = LastName,
                Bio = Bio,
                ReviewIds = ReviewIds
            };
            return publicUser;            
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
