 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 using IdentityServer4.Models;
 using IdentityServer4.Stores;
 using Raven.Client.Documents;
 using Raven.Client.Documents.Indexes;
 using Raven.Client.Documents.Operations;
 using Raven.Client.Documents.Operations.Indexes;
 using Raven.Client.Documents.Session;

 namespace WePrint.Raven
{
    public class RavenPersistedGrantStore : IPersistedGrantStore
    {
        private readonly IDocumentStore _store;
        private readonly IAsyncDocumentSession _session;

        internal const string SubjectAndClient = "PersistedGrantBySubjectAndClient";
        internal const string SubjectClientAndType = "PersistedGrantBySubjectClientAndType";

        public RavenPersistedGrantStore(IDocumentStore store)
        {
            _store = store;
            _session = store.OpenAsyncSession();
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await _session.StoreAsync(grant, grant.Key);
            await _session.SaveChangesAsync();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            return await _session.LoadAsync<PersistedGrant>(key);
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return await _session.Query<PersistedGrant>().Where(x => x.SubjectId == subjectId).ToListAsync();
        }

        public async Task RemoveAsync(string key)
        {
            _session.Delete(key);
            await _session.SaveChangesAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var op = await _store.Operations.SendAsync(
                new DeleteByQueryOperation<PersistedGrant>(SubjectAndClient,
                    x => x.SubjectId == subjectId && x.ClientId == clientId));
            await op.WaitForCompletionAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var op = await _store.Operations.SendAsync(
                new DeleteByQueryOperation<PersistedGrant>(SubjectClientAndType,
                    x => x.SubjectId == subjectId && x.ClientId == clientId && x.Type == type));
            await op.WaitForCompletionAsync();
        }
    }
}
