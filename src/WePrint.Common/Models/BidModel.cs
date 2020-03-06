using System;

namespace WePrint.Common.Models
{
    public class BidModel
    {
        public int Id { get; set; }
        public string BidderId { get; set; }
        public string JobId { get; set; }
        public float Price { get; set; }
        public TimeSpan WorkTime { get; set; }
        public string Notes { get; set; }
        public float LayerHeight { get; set; }
        public float ShellThickness { get; set; }
        public float FillPercentage { get; set; }
        public float SupportDensity { get; set; }
        public string PrinterId { get; set; }
        public MaterialType MaterialType { get; set; }
        public MaterialColor MaterialColor { get; set; }
        public FinishType Finishing { get; set; }
        public bool Accepted { get; set; }

        public void ApplyChanges(BidModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
        }
    }

    public class BidUpdateModel
    {
        public string JobId { get; set; }
        public string Id { get; set; }
        public float? Price { get; set; }
        public TimeSpan? WorkTime { get; set; }
        public string Notes { get; set; }
        public float? LayerHeight { get; set; }
        public float? ShellThickness { get; set; }
        public float? FillPercentage { get; set; }
        public float? SupportDensity { get; set; }
        public string PrinterId { get; set; }
        public MaterialType? MaterialType { get; set; }
        public MaterialColor? MaterialColor { get; set; }
        public FinishType? Finishing { get; set; }
    }
}