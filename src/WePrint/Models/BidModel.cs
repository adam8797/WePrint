using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace WePrint.Models
{
    public class BidModel : DbModel, IIdempotentDbModel
    {
        public string BidderId { get; set; }
        // We store the Job Idempotency Key here so that if it doesn't match the jobs current idemp key, we know this bid is out-of-date and no longer valid.
        public int JobIdempotencyKey { get; set; }
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
        public int IdempotencyKey { get; set; }
        public bool Accepted { get; set; }

        public void ApplyChanges(BidUpdateModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
        }
    }

    public class NewBidModel
    {
        public string JobId { get; set; }
        public int JobIdempotencyKey { get; set; }
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