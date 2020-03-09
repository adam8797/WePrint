namespace WePrint.Common.Models
{
    public class AddressModel
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }

        public AddressModel GetPublicAddress()
        {
            AddressModel returnable = this;
            returnable.StreetAddress = "";
            return returnable;
        }
    }
}