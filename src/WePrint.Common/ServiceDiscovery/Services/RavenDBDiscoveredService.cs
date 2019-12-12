using System;
using System.Collections.Generic;
using System.Text;

namespace WePrint.Common.ServiceDiscovery.Services
{
    public class RavenDBDiscoveredService : INamedDiscoveredService
    {
        public string ConfigSection => "RavenDB";

        public string DatabaseName { get; set; }

        public string CertFilePath { get; set; }

        public string CertPassword { get; set; }

        public IList<string> Hosts { get; set; }
    }
}
