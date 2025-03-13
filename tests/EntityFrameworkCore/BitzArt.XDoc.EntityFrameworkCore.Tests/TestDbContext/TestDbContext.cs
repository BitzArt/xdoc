using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BitzArt.XDoc.Tests;

public class TestDbContext(Action<TestDbContext, ModelBuilder> onModelCreating) : DbContext
{
    private static SqliteConnection _connection = null!;

    private static Lock _connectionLock = new();

    private static void CheckConnection()
    {
        if (_connection is not null) return;

        lock (_connectionLock)
        {
            if (_connection is not null) return;

            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        CheckConnection();

        optionsBuilder.UseSqlite(_connection!);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        onModelCreating.Invoke(this, modelBuilder);
    }
}
