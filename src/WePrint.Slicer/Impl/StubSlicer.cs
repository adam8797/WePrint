using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WePrint.Common;
using WePrint.Common.Slicer.Interface;
using WePrint.Common.Slicer.Models;
using WePrint.Slicer.Interface;

namespace WePrint.Slicer.Impl
{
    public class StubSlicer : ISlicer
    {
        public SliceReport Slice(string fileName, string filePath)
        {
            return new SliceReport()
            {
                FileName = fileName,
                MaterialEstimates = new[]
                {
                    new MaterialEstimate()
                    {
                        MaterialType = "PLA",
                        Unit = "g",
                        Value = 152.0f
                    },
                    new MaterialEstimate()
                    {
                        MaterialType = "Resin",
                        Unit = "ml",
                        Value = 352.0f
                    }
                },
                TimeEstimates = new []
                {
                    new TimeEstimate()
                    {
                        PrinterType = MachineType.FDM,
                        TimeSpan = TimeSpan.Parse("01:00:00")
                    },
                    new TimeEstimate()
                    {
                        PrinterType = MachineType.SLA,
                        TimeSpan = TimeSpan.Parse("00:34:00")
                    }
                }
            };
        }
    }
}
