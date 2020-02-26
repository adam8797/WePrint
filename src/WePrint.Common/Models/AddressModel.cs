namespace WePrint.Common.Models
{
    public class AddressModel : DbModel
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }

        public void ApplyChanges(AddressUpdateModel update)
        {
            ReflectionHelper.CopyPropertiesTo(update, this);
        }
    }

    public class AddressUpdateModel
    {
        public string Id { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? ZipCode { get; set; }
    }
}