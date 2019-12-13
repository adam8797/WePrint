using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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

        public async Task<T> DiscoverToAsync<T>(T instance, string service, Action<T, string[]> setUrls)
        {
            return await DiscoverToAsync(instance, service, setUrls, (t, config) => config.Bind(t));
        }

        public async Task<T> DiscoverToAsync<T>(
            T instance,
            string service,
            Action<T, string[]> setUrls,
            Action<T, IConfigurationSection> setOther)
        {
            var serviceSection = _section.GetSection(service);
            var name = serviceSection["DiscoveryName"];

            var urls = await DoDnsRequestAsync(name);
            setUrls(instance, urls);
            setOther(instance, serviceSection);

            return instance;
        }

        private async Task<T> DiscoverToAsync<T>(T t, string service) where T : IDiscoveredService
        {
            var serviceSection = _section.GetSection(service);
            serviceSection.Bind(t);
            
            var name = serviceSection["DiscoveryName"];
            var urls = await DoDnsRequestAsync(name);

            t.Hosts = urls;
            serviceSection.Bind(t);

            return t;
        }

        private async Task<string[]> DoDnsRequestAsync(string name)
        {
            try
            {
                var results = await Dns.GetHostEntryAsync(name).ConfigureAwait(false);
                return results.AddressList.Select(x => x.ToString()).ToArray();
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex);
            }
            return new string[0];
        }


    }
}