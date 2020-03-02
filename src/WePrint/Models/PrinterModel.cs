using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace WePrint.Models
{
    public class PrinterModel : DbModel
    {
        public string Name { get; set; }
        public PrinterType Type { get; set; }
        public double XMax { get; set; }
        public double YMax { get; set; }
        public double ZMax { get; set; }
        public double LayerMin { get; set; }

        public void ApplyChanges(PrinterUpdateModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
        }
    }

    public class PrinterUpdateModel
    {
        public string Name { get; set; }
        public PrinterType? Type { get; set; }
        public double? XMax { get; set; }
        public double? YMax { get; set; }
        public double? ZMax { get; set; }
        public double? LayerMin { get; set; }
    }
}