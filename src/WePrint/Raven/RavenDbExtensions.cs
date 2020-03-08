using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using WePrint.Common.Models;

namespace WePrint.Raven
{
    public static class RavenDbExtensions
    {
        public static async Task<T> AssumeLoad<T>(this IAsyncDocumentSession session, string id) where T: DbModel
        {
            var collection = session.Advanced.DocumentStore.Conventions.FindCollectionName(typeof(T));
            var obj = await session.LoadAsync<T>(collection + "/" + id);

            if (obj != null)
                obj.Id = id;

            return obj;
        }
    }
}
