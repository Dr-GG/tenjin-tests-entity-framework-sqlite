using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tenjin.Tests.EntityFramework.Sqlite.Factories
{
    public abstract class SqliteEntityFrameworkDbContextFactory<TDbContext> : IDisposable, IAsyncDisposable
        where TDbContext : DbContext
    {
        private SqliteConnection? _connection;

        public TDbContext Context
        {
            get
            {
                OpenConnection();

                return Create(GetOptions());
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();

            return ValueTask.CompletedTask;
        }

        protected abstract TDbContext Create(DbContextOptions<TDbContext> options);

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
}
