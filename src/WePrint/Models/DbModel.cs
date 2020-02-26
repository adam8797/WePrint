using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Models
{
    public abstract class DbModel: IDbModel
    {
        public string Id { get; set; }
    }
}
