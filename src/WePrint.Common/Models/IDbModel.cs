namespace WePrint.Common.Models
{
    public interface IDbModel
    {
        string Id { get; }
    }

    public interface IIdempotentDbModel : IDbModel
    {
        int IdempotencyKey { get; set; }
    }

    public static class DbModelExtensions
    {
        public static IIdempotentDbModelKey GetKey(this IIdempotentDbModel model)
        {
            return new IIdempotentDbModelKey(model.Id, model.IdempotencyKey);
        }
    }

    public class IIdempotentDbModelKey
    {
        public string Id { get; private set; }
        public int IdempotencyKey { get; private set; }
        public IIdempotentDbModelKey(string id, int idempotencyKey)
        {
            Id = id;
            IdempotencyKey = idempotencyKey;
        }

        public EIdempotencyMatchType Match(IIdempotentDbModel model)
        {
            if (model.Id != Id)
                return EIdempotencyMatchType.Unmatched;
            if (model.IdempotencyKey != IdempotencyKey)
                return EIdempotencyMatchType.Expired;
            return EIdempotencyMatchType.Matched;
        }
    }

    public enum EIdempotencyMatchType
    {
        Matched,
        Expired,
        Unmatched
    }
}
