using System.Globalization;
using GeoCop.Api.Models;
using Itenso.TimePeriod;
using Stac;
using Stac.Api.Interfaces;
using Stac.Api.WebApi.Services;
using Stac.Collection;

namespace GeoCop.Api.StacServices
{
    public class StacConverter
    {
        public StacConverter(IStacLinker stacLinker, IStacApiContextFactory httpStacApiContextFactory)
        {
            StacLinker = stacLinker;
            StacApiContext = httpStacApiContextFactory.Create();
        }

        private IStacLinker StacLinker { get; }
        private IStacApiContext StacApiContext { get; }

        public StacCollection ToStacCollection(Operat operat)
        {
            var collectionId = operat.Name + "_" + operat.Id;
            var items = operat.Deliveries.Select(d => ToStacItem(d, collectionId)).ToDictionary(i => i.Links.Where(l => l.RelationshipType.ToLower() == "self").First().Uri);
            if (items.Values.Count == 0)
            {
                var defaultExtent = Extent.GetDefault();
                var extent = new StacExtent(new StacSpatialExtent(defaultExtent.XMin, defaultExtent.YMin, defaultExtent.XMax, defaultExtent.YMax), new StacTemporalExtent(DateTime.Parse("2018-01-01T00:00:00Z", CultureInfo.InvariantCulture).ToUniversalTime(), DateTime.Now.ToUniversalTime()));
                return new StacCollection(collectionId, operat.Description, extent, null, null);
            }
            else
            {
                var collection = StacCollection.Create(collectionId, operat.Description, items);
                return collection;
            }
        }

        public StacItem ToStacItem(Delivery delivery, string parentId)
        {
            var stacId = delivery.Name + "_" + delivery.Id;

            var item = new StacItem(delivery.Name + "_" + delivery.Id, delivery.Geometry)
            {
                Collection = parentId,
                Description = delivery.Description,
            };

            item.DateTime = new TimePeriodChain();
            item.DateTime.Setup(delivery.UploadDate, delivery.UploadDate);

            var assets = delivery.Files.Select(file => ToStacAsset(file, item)).ToDictionary(asset => asset.Title);
            item.Assets.AddRange(assets);

            StacLinker.Link(item, StacApiContext);

            return item;
        }

        public StacAsset ToStacAsset(Models.File file, IStacObject parent)
        {
            return new StacAsset(parent, new Uri(file.Path), file.Roles.Select(r => r.ToString()), file.Name, file.ContentType);
        }
    }
}
