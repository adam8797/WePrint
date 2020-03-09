namespace WePrint.Common.Models
{
    public abstract class DbModel: IDbModel
    {
        public string Id { get; set; } = "";
    }
}
