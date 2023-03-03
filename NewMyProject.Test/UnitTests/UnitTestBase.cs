using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NewMyProject.Data;
using System;

namespace NewMyProject.Test
{
    public class UnitTestBase
    {
        protected EfContext CreateInMemoryContext(Action<EfContext> dbSeeder = null)
        {
            var options = new DbContextOptionsBuilder<EfContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => 
                                  x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using (var context = new EfContext(options))
            {
                dbSeeder?.Invoke(context);
                context.SaveChangesAsync().GetAwaiter().GetResult();
            }

            return new EfContext(options);
        }
    }
}
