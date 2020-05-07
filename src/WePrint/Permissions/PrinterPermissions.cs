using System.Threading.Tasks;
using WePrint.Models;

namespace WePrint.Permissions
{
    public class PrinterPermissions : IPermissionProvider<printer, printer_create_model>
    {
        public async ValueTask<bool> AllowWrite(user user, printer data) => true;

        public async ValueTask<bool> AllowRead(user user, printer data) => data.owner == user;

        public async ValueTask<bool> AllowCreate(user user, printer_create_model data) => true;
    }

    public class OrganizationPermissions : IPermissionProvider<organization, organization_create_model>
    {
        public async ValueTask<bool> AllowWrite(user user, organization data) => true;
        
        public async ValueTask<bool> AllowRead(user user, organization data) => true;

        public async ValueTask<bool> AllowCreate(user user, organization_create_model data) => user.organization == null;
    }

    public class ProjectPermissions : IPermissionProvider<project, project_create_model>
    {
        public async ValueTask<bool> AllowWrite(user user, project data) => user.organization == data.organization;

        public async ValueTask<bool> AllowRead(user user, project data) => true;

        public async ValueTask<bool> AllowCreate(user user, project_create_model data) => user.organization != null;
    }

    public class PledgePermissions : IPermissionProvider<pledge, pledge_create_model>
    {
        public async ValueTask<bool> AllowWrite(user user, pledge data)
        {
            return data.maker == user || data.project.organization.users.Contains(user);
        }

        public async ValueTask<bool> AllowRead(user user, pledge data)
        {
            return true;
        }

        public async ValueTask<bool> AllowCreate(user user, pledge_create_model data)
        {
            // This may need some refactoring to support
            return true;
        }
    }
}
