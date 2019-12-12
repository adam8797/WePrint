using System;
using System.Collections.Generic;
using System.Text;

namespace WePrint.Common.ServiceDiscovery.Services
{
    public class RabbitMQDiscoveredService : INamedDiscoveredService
    {
        public string ConfigSection => "RabbitMQ";

        public IList<string> Hosts { get; set; }

        public string Queue { get; set; }
    }
}
