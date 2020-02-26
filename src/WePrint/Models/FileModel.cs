using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace WePrint.Models
{
    public class FileModel : DbModel
    {
        public bool PreProcessed { get; set; }
        public int EstimatedPrintTime { get; set; } // This is all pending how we end up doing the slicing, might need to be a  list of saved examples linked to printers. Dict<Printer, Tuple<int, float>> maybe?
        public float EstimatedPrintVolume { get; set; }
        public string JobId { get; set; }

        // None of these fields can be reasonably updated by a client, only by the server.
        //public void ApplyChanges(FileUpdateModel update)
        //{
        //    ReflectionHelper.CopyPropertiesTo(update, this);
        //}
    }
    
}