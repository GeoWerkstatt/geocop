using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GeoCop.Api.Test
{
    public static class TestDbContextFactory
    {
        private static readonly TrackChangesInterceptor trackChangesInterceptor = new TrackChangesInterceptor();
        private static string ConnectionString { get; } = "Server=localhost;Port=5432;Database=geocop-test;User Id=HAPPYWALK;Password=SOMBERSPORK;";

        private static DbContextOptionsBuilder<Context> OptionsBuilder { get; set; }

        public static void SeedInitialContext()
        {
            OptionsBuilder =
                new DbContextOptionsBuilder<Context>()
                .UseNpgsql(ConnectionString, o =>
                {
                    o.UseNetTopologySuite();
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

            using var setupContext = new Context(OptionsBuilder.Options);
            setupContext.Database.EnsureDeleted();
            setupContext.Database.EnsureCreated();
            setupContext.SeedTestData();
            setupContext.SaveChanges();
        }

        public static Context GetContext()
        {
            using var context = new Context(OptionsBuilder.Options);
            trackChangesInterceptor.ResetContext(context);

            return new Context(OptionsBuilder.AddInterceptors(trackChangesInterceptor).Options);
        }
    }
}
