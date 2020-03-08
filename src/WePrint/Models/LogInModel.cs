using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Models
{
    public class LogInModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public bool Remember { get; set; }
    }
}
