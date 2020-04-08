﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WePrint.ViewModels
{
    public class PrinterViewModel
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Name { get; set; }

        public PrinterType Type { get; set; }

        public int XMax { get; set; } //mm

        public int YMax { get; set; } //mm

        public int ZMax { get; set; }  //mm

        public decimal LayerMin { get; set; } //mm
    }
}
