using System.Threading.Tasks;
using WePrint.Data;
using WePrint.Models;

namespace WePrint.Permissions
{
    public class PrinterPermissions : IPermissionProvider<Printer, PrinterCreateModel>
    {
        public async ValueTask<bool> AllowWrite(User user, Printer data) => true;

        public async ValueTask<bool> AllowRead(User user, Printer data) => data.Owner == user;

        public async ValueTask<bool> AllowCreate(User user, PrinterCreateModel data) => true;
    }

    public class OrganizationPermissions : IPermissionProvider<Organization, OrganizationCreateModel>
    {
        public async ValueTask<bool> AllowWrite(User user, Organization data) => true;
        
        public async ValueTask<bool> AllowRead(User user, Organization data) => true;

        public async ValueTask<bool> AllowCreate(User user, OrganizationCreateModel data) => user.Organization == null;
    }

    public class ProjectPermissions : IPermissionProvider<Project, ProjectCreateModel>
    {
        public async ValueTask<bool> AllowWrite(User user, Project data) => user.Organization == data.Organization;

        public async ValueTask<bool> AllowRead(User user, Project data) => true;

        public async ValueTask<bool> AllowCreate(User user, ProjectCreateModel data) => user.Organization != null;
    }

    public class UpdatePermissions : IPermissionProvider<ProjectUpdate, ProjectUpdateCreateModel>
    {
        public async ValueTask<bool> AllowCreate(User user, ProjectUpdateCreateModel data) => true;

        public async ValueTask<bool> AllowRead(User user, ProjectUpdate data) => true;

        public async ValueTask<bool> AllowWrite(User user, ProjectUpdate data) => user.Organization == data.Project.Organization;
    }

    public class PledgePermissions : IPermissionProvider<Pledge, PledgeCreateModel>
    {
        public async ValueTask<bool> AllowWrite(User user, Pledge data)
        {
            return data.Maker == user || data.Project.Organization.Users.Contains(user);
        }

        public async ValueTask<bool> AllowRead(User user, Pledge data)
        {
            return true;
        }

        public async ValueTask<bool> AllowCreate(User user, PledgeCreateModel data)
        {
            // This may need some refactoring to support
            return true;
        }
    }
}
