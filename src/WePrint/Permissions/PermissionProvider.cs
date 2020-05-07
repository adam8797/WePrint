using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WePrint.Models;

namespace WePrint.Permissions
{
    public static class PermissionProvider
    {
        public static void RegisterPermissions(this IServiceCollection services)
        {
            var types = typeof(PermissionProvider).Assembly.GetTypes().Where(x =>
                x.GetInterfaces().Any(y =>
                {
                    if (y.IsGenericType)
                        return y.GetGenericTypeDefinition() == typeof(IPermissionProvider<,>);
                    return false;
                }))
                .Where(x => !x.IsGenericType)
                .ToList();

            foreach (var type in types)
            {
                var providerInterface = type.GetInterfaces().SingleOrDefault(x => x.GetGenericTypeDefinition() == typeof(IPermissionProvider<,>));
                services.AddTransient(providerInterface, type);
            }
        }
    }

    public interface IPermissionProvider<in TData, in TCreate>
    {
        ValueTask<bool> AllowWrite(user user, TData data);
        ValueTask<bool> AllowRead(user user, TData data);
        ValueTask<bool> AllowCreate(user user, TCreate data);
    }

    public sealed class DefaultPermissionProvider<TData, TCreate> : IPermissionProvider<TData, TCreate>
    {
        public async ValueTask<bool> AllowWrite(user user, TData data) => true;

        public async ValueTask<bool> AllowRead(user user, TData data) => true;

        public async ValueTask<bool> AllowCreate(user user, TCreate data) => true;
    }
}
