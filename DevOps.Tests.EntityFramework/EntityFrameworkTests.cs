using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace DevOps.Tests.EntityFramework
{
    public static class EntityFrameworkTests
    {
        public static void Run<TDbContext>(Action<TDbContext> doWork, Action<TDbContext> verifyResults)
            where TDbContext : DbContext
        {
            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/sqlite
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                connection.Open();
                var options = GetOptions(connection);
                using (var context = GetContext(options)) context.Database.EnsureCreated();
                using (var context = GetContext(options)) doWork(context);
                using (var context = GetContext(options)) verifyResults(context);
            }

            TDbContext GetContext(DbContextOptions<TDbContext> options)
                => Activator.CreateInstance(typeof(TDbContext), options) as TDbContext;

            DbContextOptions<TDbContext> GetOptions(DbConnection connection)
                => new DbContextOptionsBuilder<TDbContext>()
                    .UseSqlite(connection)
                    .Options;
        }
    }
}
