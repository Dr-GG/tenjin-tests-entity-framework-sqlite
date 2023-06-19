using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tenjin.Tests.EntityFramework.Sqlite.Factories;

/// <summary>
/// A base class that enables one to use Sqlite in-memory database connections with an EF.Core DbContext instance.
/// </summary>
public abstract class SqliteEntityFrameworkDbContextFactory<TDbContext> : IDisposable, IAsyncDisposable
    where TDbContext : DbContext
{
    private SqliteConnection? _connection;

    /// <summary>
    /// The DbContext instance to be used.
    /// </summary>
    public TDbContext Context
    {
        get
        {
            OpenConnection();

            return Create(GetOptions());
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Creates the appropriate DbContext instance.
    /// </summary>
    protected abstract TDbContext Create(DbContextOptions<TDbContext> options);

    /// <summary>
    /// Disposes of any resources within the connection.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        _connection?.Close();
        _connection?.Dispose();
    }

    private DbContextOptions<TDbContext> GetOptions()
    {
        if (_connection == null)
        {
            throw new InvalidOperationException("Initialise SQLite connection first");
        }

        return new DbContextOptionsBuilder<TDbContext>()
            .UseSqlite(_connection)
            .Options;
    }

    private void OpenConnection()
    {
        if (_connection != null)
        {
            return;
        }

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        using var createContext = Create(GetOptions());

        if (!createContext.Database.EnsureCreated())
        {
            throw new InvalidOperationException("Sqlite memory provider failed");
        }
    }
}