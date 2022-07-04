using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Tenjin.Tests.EntityFramework.Sqlite.Tests.Database;
using Tenjin.Tests.EntityFramework.Sqlite.Tests.Factories;
using Tenjin.Tests.EntityFramework.Sqlite.Tests.Models;

namespace Tenjin.Tests.EntityFramework.Sqlite.Tests.FactoriesTests;

[TestFixture]
public class SqliteEntityFrameworkDbContextFactoryTests
{
    [Test]
    public async Task CreateDbContext_PerformInsertOperations_BehavesAsExpected()
    {
        await using var factory = new TestSqliteDbContextFactory();
        await using var dbContext = factory.Context;

        AddDefaultData(dbContext);

        ShouldExist(dbContext, 1, "First Name 1", "Last Name 1");
        ShouldExist(dbContext, 2, "First Name 2", "Last Name 2");
        ShouldExist(dbContext, 3, "First Name 3", "Last Name 3");
    }

    [Test]
    public async Task CreateDbContext_PerformUpdateOperations_BehavesAsExpected()
    {
        await using var factory = new TestSqliteDbContextFactory();
        await using var dbContext = factory.Context;

        AddDefaultData(dbContext);
        SwapFirstNameAndLastName(dbContext, 1);
        SwapFirstNameAndLastName(dbContext, 3);

        ShouldExist(dbContext, 1, "Last Name 1" , "First Name 1");
        ShouldExist(dbContext, 2, "First Name 2", "Last Name 2");
        ShouldExist(dbContext, 3, "Last Name 3" , "First Name 3");
    }

    [Test]
    public async Task CreateDbContext_PerformDeleteOperations_BehavesAsExpected()
    {
        await using var factory = new TestSqliteDbContextFactory();
        await using var dbContext = factory.Context;

        AddDefaultData(dbContext);
        DeleteData(dbContext, 1);

        ShouldExist(dbContext, 2, "First Name 2", "Last Name 2");
        ShouldExist(dbContext, 3, "First Name 3", "Last Name 3");

        ShouldNotExist(dbContext, 1);
    }

    [Test]
    public async Task CreateDbContext_WhenUsingTwoDbContexts_DoesNotInterfere()
    {
        const string dbContext1Suffix1 = "_dbContext1";
        const string dbContext1Suffix2 = "_dbContext2";

        await using var factory1 = new TestSqliteDbContextFactory();
        await using var factory2 = new TestSqliteDbContextFactory();
        await using var dbContext1 = factory1.Context;
        await using var dbContext2 = factory2.Context;

        AddDefaultData(dbContext1, dbContext1Suffix1);
        AddDefaultData(dbContext2, dbContext1Suffix2);

        ShouldExist(dbContext1, 1, $"First Name 1{dbContext1Suffix1}", $"Last Name 1{dbContext1Suffix1}");
        ShouldExist(dbContext1, 2, $"First Name 2{dbContext1Suffix1}", $"Last Name 2{dbContext1Suffix1}");
        ShouldExist(dbContext1, 3, $"First Name 3{dbContext1Suffix1}", $"Last Name 3{dbContext1Suffix1}");

        ShouldExist(dbContext2, 1, $"First Name 1{dbContext1Suffix2}", $"Last Name 1{dbContext1Suffix2}");
        ShouldExist(dbContext2, 2, $"First Name 2{dbContext1Suffix2}", $"Last Name 2{dbContext1Suffix2}");
        ShouldExist(dbContext2, 3, $"First Name 3{dbContext1Suffix2}", $"Last Name 3{dbContext1Suffix2}");

        ShouldNotExist(dbContext1, 4);
        ShouldNotExist(dbContext1, 5);
        ShouldNotExist(dbContext1, 6);

        ShouldNotExist(dbContext2, 4);
        ShouldNotExist(dbContext2, 5);
        ShouldNotExist(dbContext2, 6);
    }

    private static void AddDefaultData(TestSqliteDbContext dbContext, string suffix = "")
    {
        AddPerson(dbContext, $"First Name 1{suffix}", $"Last Name 1{suffix}");
        AddPerson(dbContext, $"First Name 2{suffix}", $"Last Name 2{suffix}");
        AddPerson(dbContext, $"First Name 3{suffix}", $"Last Name 3{suffix}");
    }

    private static void SwapFirstNameAndLastName(TestSqliteDbContext dbContext, int id)
    {
        var person = dbContext.Persons.Single(p => p.Id == id);
            
        (person.FirstName, person.LastName) = (person.LastName, person.FirstName);

        dbContext.SaveChanges();
    }

    private static void DeleteData(TestSqliteDbContext dbContext, int id)
    {
        var person = dbContext.Persons.Single(p => p.Id == id);

        dbContext.Persons.Remove(person);
        dbContext.SaveChanges();
    }

    private static void ShouldNotExist(TestSqliteDbContext dbContext, int id)
    {
        var person = dbContext.Persons.SingleOrDefault(p => p.Id == id);

        Assert.IsNull(person);
    }

    private static void ShouldExist(
        TestSqliteDbContext dbContext,
        int id,
        string firstName,
        string lastName)
    {
        var person = dbContext.Persons.SingleOrDefault(p => p.Id == id);

        Assert.IsNotNull(person);
        Assert.AreEqual(firstName, person!.FirstName);
        Assert.AreEqual(lastName, person.LastName);
    }

    private static void AddPerson(
        TestSqliteDbContext dbContext,
        string firstName,
        string lastName)
    {
        var personModel = new PersonModel
        {
            FirstName = firstName,
            LastName = lastName
        };

        dbContext.Persons.Add(personModel);
        dbContext.SaveChanges();
    }
}