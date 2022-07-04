using Microsoft.EntityFrameworkCore;
using Tenjin.Tests.EntityFramework.Sqlite.Tests.Models;

namespace Tenjin.Tests.EntityFramework.Sqlite.Tests.Database;

public class TestSqliteDbContext : DbContext
{
    public TestSqliteDbContext(DbContextOptions options) : base(options)
    { }

    public DbSet<PersonModel> Persons { get; set; } = null!;
}