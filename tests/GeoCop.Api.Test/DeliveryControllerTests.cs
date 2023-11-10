using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using GeoCop.Api;
using GeoCop.Api.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MyProject.Tests
{
    [SimpleJob(RuntimeMoniker.Net70)]
    public class DeliveryControllerTests
    {
        public static string ConnectionString { get; } = "Server=localhost;Port=5432;Database=geocop-test;User Id=HAPPYWALK;Password=SOMBERSPORK;";

        public static DbContextOptions<Context> Options { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Options =
                new DbContextOptionsBuilder<Context>()
                .UseNpgsql(ConnectionString, o =>
                {
                    o.UseNetTopologySuite();
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })
                .Options;

            using var context = new Context(Options);
            context.Database.EnsureCreated();

            TestDbContextFactory.SeedInitialContext();
        }

        [Benchmark]
        public void DeleteCreate()
        {
            using var context = new Context(Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Benchmark]
        public void SeedSaveChanges()
        {
            using var context = new Context(Options);
            context.SeedTestData();
            context.SaveChanges();
        }

        [Benchmark]
        public void SeedOnly()
        {
            using var context = new Context(Options);
            context.SeedTestData();
        }

        [Benchmark]
        public void CreateDisposeContext()
        {
            using var context = new Context(Options);
        }

        [Benchmark]
        public void TransactionRollback()
        {
            using var context = new Context(Options);
            using var transaction = context.Database.BeginTransaction();
            context.SeedTestData();
            context.SaveChanges();
            transaction.Rollback();
        }

        [Benchmark]
        public void RollbackContext()
        {
            using var context = TestDbContextFactory.GetContext();
            context.SeedTestData();
        }
    }
}
