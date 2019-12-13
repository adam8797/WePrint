using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WePrint.Common.ServiceDiscovery
{
    public class DNSServiceDiscovery : IServiceDiscovery
    {
        private IConfigurationSection _section;

        public DNSServiceDiscovery(IConfiguration configuration)
        {
            _section = configuration.GetSection("Services");
        }

        public DNSServiceDiscovery(IConfigurationSection section)
        {
            _section = section;
        }

        public async Task<T> DiscoverAsync<T>(string service) where T: IDiscoveredService, new()
        {
            return await DiscoverToAsync(new T(), service);
        }

        public async Task<T> DiscoverAsync<T>() where T : INamedDiscoveredService, new()
        {
            var t = new T();
            return await DiscoverToAsync(t, t.ConfigSection);
        }

        private async Task<T> DiscoverToAsync<T>(T t, string service) where T : IDiscoveredService
        {
            var serviceSection = _section.GetSection(service);
            serviceSection.Bind(t);
            var name = serviceSection["DiscoveryName"];

            try
            {
                var results = await Dns.GetHostEntryAsync(name).ConfigureAwait(false);
                t.Hosts = results.AddressList.Select(x => x.ToString()).ToList();
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex);
            }

            return t;
        }
    }
}