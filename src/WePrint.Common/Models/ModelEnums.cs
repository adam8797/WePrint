namespace WePrint.Common.Models
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