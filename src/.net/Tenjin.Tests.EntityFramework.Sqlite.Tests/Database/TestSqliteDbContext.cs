using Microsoft.EntityFrameworkCore;
using Tenjin.Tests.EntityFramework.Sqlite.Tests.Models;

namespace Tenjin.Tests.EntityFramework.Sqlite.Tests.Database;

public class TestSqliteDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<PersonModel> Persons { get; set; } = null!;
}