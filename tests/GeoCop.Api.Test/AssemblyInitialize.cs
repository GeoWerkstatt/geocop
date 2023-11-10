using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCop.Api.Test
{
    public class AssemblyInitialize
    {
        //private static string ConnectionString { get; } = "Server=localhost;Port=5432;Database=geocop;User Id=HAPPYWALK;Password=SOMBERSPORK;";
        //private static DbContextOptions<Context> DbContextOptions { get; } =
        //    new DbContextOptionsBuilder<Context>()
        //        .UseNpgsql(ConnectionString, o =>
        //        {
        //            o.UseNetTopologySuite();
        //            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        //        })
        //        .AddInterceptors(new Npgsql.NpgsqlTransaction.TransactionInterceptor())
        //        .Options;

        //[AssemblyInitialize]
        //public void Initialize()
        //{
        //    using var context = new Context(DbContextOptions);
        //    context.Database.EnsureDeleted();
        //    context.Database.EnsureCreated();
        //    context.SeedTestData();
        //    context.SaveChanges();
        //}
    }
}
