namespace WePrint.Common.Slicer.Models
{
    public class MaterialEstimate
    {
        public string MaterialType { get; set; } 

        public string Unit { get; set; }

        public float Value { get; set; }

        public override string ToString()
        {
            return $"{Value:F2} {Unit} of {MaterialType}";
        }
    }
}
