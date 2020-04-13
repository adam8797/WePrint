using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Data
{
    public interface IIdentifiable<T> where T : struct
    {
        T Id { get; set; }
    }
}
