using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.Indexes;

namespace WePrint.Raven
{
    public static class RavenDBInitialization
    {
        public static void SetupApplicationDependencies(this IDocumentStore store)
        {
            SetupIndexes(store);
        }

        private static void SetupIndexes(IDocumentStore store)
        {
            if (store.Maintenance.Send(new GetIndexOperation(RavenPersistedGrantStore.SubjectAndClient)) == null)
            {
                var definition = new IndexDefinitionBuilder<PersistedGrant>
                {
                    Map = grants => from grant in grants
                        select new
                        {
                            grant.SubjectId,
                            grant.ClientId
                        },
                }.ToIndexDefinition(store.Conventions);

                definition.Name = RavenPersistedGrantStore.SubjectAndClient;

                store.Maintenance.Send(new PutIndexesOperation(definition));
            }

            if (store.Maintenance.Send(new GetIndexOperation(RavenPersistedGrantStore.SubjectClientAndType)) == null)
            {
                var definition = new IndexDefinitionBuilder<PersistedGrant>
                {
                    Map = grants => from grant in grants
                        select new
                        {
                            grant.SubjectId,
                            grant.ClientId,
                            grant.Type
                        },
                }.ToIndexDefinition(store.Conventions);

                definition.Name = RavenPersistedGrantStore.SubjectClientAndType;

                store.Maintenance.Send(new PutIndexesOperation(definition));
            }
        }
    }
}
