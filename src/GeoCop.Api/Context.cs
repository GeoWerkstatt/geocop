using Bogus;
using GeoCop.Api.Models;
using GeoCop.Api.StacServices;
using Stac;
using System.Net.Mime;

namespace GeoCop.Api
{
    /// <summary>
    /// Test context.
    /// </summary>
    public class Context
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public Context(StacConverter stacConverter)
        {
            StacConverter = stacConverter;
            GetOperats();
        }

        private StacConverter StacConverter { get; }

        public Dictionary<string, StacCollection> Collections { get; private set; } = new Dictionary<string, StacCollection>();

        // public Dictionary<string, StacItem> Items { get; private set; } = new Dictionary<string, StacItem>();
        private List<Models.File> GetFiles()
        {
            ContentType ct = new ContentType();
            ct.MediaType = MediaTypeNames.Text.Plain;
            var fileIds = 0;
            var fakeFiles = new Faker<Models.File>()
                .StrictMode(true)
                .RuleFor(c => c.Id, f => fileIds++)
                .RuleFor(c => c.Name, f => f.Random.Word())
                .RuleFor(c => c.Path, f => "C://" + f.System.FilePath().Trim('/').OrDefault(f, 0.1f, "obsolete" + f.System.FilePath()))
                .RuleFor(c => c.Roles, f => new List<FileRole>() { (FileRole)(fileIds - 1) })
                .RuleFor(c => c.ContentType, f => ct);
            Models.File SeededFile(int seed) => fakeFiles.UseSeed(seed).Generate();
            return Enumerable.Range(1, 2).Select(SeededFile).ToList();
        }

        private List<Delivery> GetDeliveries(Operat operat)
        {
            var deliveryIds = 1;
            Random random = new Random();
            var deliveryRange = Enumerable.Range(deliveryIds, random.Next(1, 10));
            var fakeDeliveries = new Faker<Delivery>()
                .StrictMode(true)
                .RuleFor(c => c.Id, f => random.Next(1000) + deliveryIds++)
                .RuleFor(c => c.Name, f => f.Random.Word())
                .RuleFor(c => c.Description, f => f.Random.Words())
                .RuleFor(c => c.Operat, f => operat)
                .RuleFor(c => c.Geometry, f => Extent.GetDefault().AsGeometry())
                .RuleFor(c => c.UploadDate, f => f.Date.Past())
                .RuleFor(c => c.Files, f => GetFiles());
            Delivery SeededDelivery(int seed) => fakeDeliveries.UseSeed(seed).Generate();
            var deliveries = deliveryRange.Select(SeededDelivery).ToList();
            return deliveries;
        }

        public List<Operat> GetOperats()
        {
            var operatIds = 1;
            var operatRange = Enumerable.Range(operatIds, 10);
            var fakeOperats = new Faker<Operat>()
                .StrictMode(true)
                .RuleFor(c => c.Id, f => operatIds++)
                .RuleFor(c => c.Name, f => f.Random.Word())
                .RuleFor(c => c.Description, f => f.Random.Words())
                .RuleFor(c => c.Deliveries, f => new List<Delivery>())
                .RuleFor(c => c.Extent, f => new Extent { XMin = 45.82F, YMin = 5.96F, XMax = 47.81F, YMax = 10.49F });
            Operat SeededOperat(int seed) => fakeOperats.UseSeed(seed).Generate();
            var operats = operatRange.Select(SeededOperat).ToList();
            operats.ForEach(operat =>
            {
                operat.Deliveries = GetDeliveries(operat);
                var collection = StacConverter.ToStacCollection(operat);
                Collections.Add(collection.Id, collection);
            });
            return operats;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
