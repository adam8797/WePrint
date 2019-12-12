using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WePrint.Common.ServiceDiscovery
{
    public interface IServiceDiscovery
    {
        Task<T> DiscoverAsync<T>(string service) where T : IDiscoveredService, new();
        Task<T> DiscoverAsync<T>() where T : INamedDiscoveredService, new();

    }
}