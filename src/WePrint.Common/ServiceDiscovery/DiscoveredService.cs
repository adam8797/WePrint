using System.Collections.Generic;

namespace WePrint.Common.ServiceDiscovery
{
    public interface IDiscoveredService
    {
        IList<string> Hosts { get; set; }
    }

    public interface INamedDiscoveredService : IDiscoveredService
    {
        string ConfigSection { get; }
    }
}
