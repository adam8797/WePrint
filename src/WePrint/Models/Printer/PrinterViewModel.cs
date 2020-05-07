using System;

namespace WePrint.Models
{
    public class printer_view_model
    {
        public Guid id { get; set; }

        public Guid owner_id { get; set; }

        public string name { get; set; }

        public PrinterType type { get; set; }

        public int x_max { get; set; } //mm

        public int y_max { get; set; } //mm

        public int z_max { get; set; }  //mm

        public decimal layer_min { get; set; } //mm

        public bool deleted { get; set; }
    }

    public class printer_create_model
    {
        public string name { get; set; }

        public PrinterType type { get; set; }

        public int x_max { get; set; } //mm

        public int y_max { get; set; } //mm

        public int z_max { get; set; }  //mm

        public decimal layer_min { get; set; } //mm
    }
}
