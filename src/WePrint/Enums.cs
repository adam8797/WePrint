using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WePrint
{
    [JsonConverter(typeof(StringEnumConverter))]
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PrinterType
    {
        SLA,
        FDM,
        LaserCut
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MaterialType
    {
        ABS,
        PLA,
        Resin,
        Polycarbonate,
        Flexible
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MaterialColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Clear,
        Any
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FinishType
    {
        Sanding,
        Varnish,
        Priming,
        Painting,
        None
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PledgeStatus
    {
        NotStarted,
        InProgress,
        Shipped,
        Finished
    }
}