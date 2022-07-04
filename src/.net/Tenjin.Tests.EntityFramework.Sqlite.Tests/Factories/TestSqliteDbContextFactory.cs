using Microsoft.EntityFrameworkCore;
using Tenjin.Tests.EntityFramework.Sqlite.Factories;
using Tenjin.Tests.EntityFramework.Sqlite.Tests.Database;

namespace Tenjin.Tests.EntityFramework.Sqlite.Tests.Factories;

internal class TestSqliteDbContextFactory : SqliteEntityFrameworkDbContextFactory<TestSqliteDbContext>
{
    protected override TestSqliteDbContext Create(DbContextOptions<TestSqliteDbContext> options)
    {
        return new TestSqliteDbContext(options);
    }
}