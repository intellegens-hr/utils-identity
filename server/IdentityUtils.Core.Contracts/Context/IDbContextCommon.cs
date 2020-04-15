using System.Threading;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Contracts.Context
{
    /// <summary>
    /// If service need to use specific part of context (e.g. IUserContext or ITenantCOntext), these are common features
    /// each interface should have in order for service to do context actions such as save changes.
    /// </summary>
    public interface IDbContextCommon
    {
        int SaveChanges(bool acceptAllChangesOnSuccess);

        int SaveChanges();

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}