using Microsoft.EntityFrameworkCore;
using System;

namespace IdentityUtils.Core.Services.Tests.Setup
{
    internal class DisposableContextService<T> : IDisposable
    {
        internal TestDbContext TestDbContext { get; }
        internal T Service { get; }

        internal DisposableContextService(Func<TestDbContext, T> serviceGetter)
        {
            TestDbContext = new TestDbContext();
            Service = serviceGetter(TestDbContext);

            TestDbContext.Database.OpenConnection();
            TestDbContext.Database.Migrate();
        }

        public void Dispose()
        {
            TestDbContext.Database.CloseConnection();
        }
    }
}