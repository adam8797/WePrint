using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.Models
{
    public enum JobStatus
    {
        PendingOpen,
        BiddingOpen,
        BiddingClosed,
        BidSelected,
        PrintComplete,
        Shipped,
        Received,
        Closed,
        Cancelled
    }

    public enum PrinterType
    {
        SLA,
        FDM,
        LaserCut
    }

    public enum MaterialType
    {
        ABS,
        PLA,
        Resin,
        Polycarbonate,
        Flexible
    }

    public enum MaterialColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Clear,
        Any
    }

    public enum FinishType
    {
        Sanding,
        Varnish,
        Priming,
        Painting
    }
}