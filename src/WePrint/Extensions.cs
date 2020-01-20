using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents;
using System.Linq;
using WePrint.Data;

namespace WePrint
{
    public static class Extensions
    {
        public static IDocumentStore EnsureExists(this IDocumentStore store)
        {
            try
            {
                using (var dbSession = store.OpenSession())
                {
                    dbSession.Query<WePrintUser>().Take(0).ToList();
                }
            }
            catch (Raven.Client.Exceptions.Database.DatabaseDoesNotExistException)
            {
                store.Maintenance.Server.Send(new Raven.Client.ServerWide.Operations.CreateDatabaseOperation(new Raven.Client.ServerWide.DatabaseRecord
                {
                    DatabaseName = store.Database
                }));
            }

            return store;
        }

        public static bool IsTesting(this IWebHostEnvironment env)
        {
            return env.IsEnvironment("Testing");
        }
    }
}