using System.Collections.Generic;

namespace WePrint.Common.ServiceDiscovery
{
    public interface IDiscoveredService
    {
        string[] Hosts { get; set; }
    }

    public interface INamedDiscoveredService : IDiscoveredService
    {
        string ConfigSection { get; }
    }
}
