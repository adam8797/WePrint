using System;
using System.Collections.Generic;
using Raven.Identity;

namespace WePrint.Common.Models
{
    public sealed class ApplicationUser : IdentityUser, IDbModel
    {
        public ApplicationUser()
        {
            // So we're generating a GUID on the client side.
            // This is a **BAD** idea, however its a quick fix.
            // The more involved way would be to rewrite the RavenDB.Identity.UserStore 
            Id = Guid.NewGuid().ToString();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<PrinterModel> Printers { get; set; }
        public string Bio { get; set; }
        public List<string> ReviewIds { get; set; }
        public AddressModel Address { get; set; }

        public PublicApplicationUserModel GetPublicModel()
        {
            var model = new PublicApplicationUserModel();
            ReflectionHelper.CopyPropertiesTo(this, model);
            return model;
        }
    }

    public class PublicApplicationUserModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string Id { get; set; }
    }
}
