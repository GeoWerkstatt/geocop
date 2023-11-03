using Itenso.TimePeriod;
using Stac;
using Stac.Collection;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using static Stac.Api.Interfaces.ILinkValues;

namespace GeoCop.Api.Models
{
    /// <summary>
    /// Represents to a STAC collection.
    /// </summary>
    public class Operat
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Extent Extent { get; set; }
        public List<Delivery> Deliveries { get; set; } = new List<Delivery>();

        // public StacCollection ConvertToStacCollection()
        // {
        //     var stacId = Name + "_" + Id;
        //     var items = Deliveries.Select(d => d.ConvertToStacItem(stacId, Extent.AsGeometry())).ToDictionary(i => i.Links.First(l => l.RelationshipType == "Self").Uri);

        //     if (items.Values.Count == 0)
        //     {
        //         var extent = new StacExtent(new StacSpatialExtent(Extent.XMin, Extent.YMin, Extent.XMax, Extent.YMax), new StacTemporalExtent(DateTime.Parse("2018-01-01T00:00:00Z", CultureInfo.InvariantCulture).ToUniversalTime(), DateTime.Now.ToUniversalTime()));
        //         return new StacCollection(stacId, Description, extent, null, null);
        //     }
        //     else
        //     {
        //         var collection = StacCollection.Create(stacId, Description, items);
        //         return collection;
        //     }
        // }
    }

    public class Extent
    {
        public float XMin { get; set; }
        public float YMin { get; set; }
        public float XMax { get; set; }
        public float YMax { get; set; }

        public GeoJSON.Net.Geometry.Polygon AsGeometry()
        {
            var polygon = new GeoJSON.Net.Geometry.Polygon(new List<GeoJSON.Net.Geometry.LineString>()
            {
                new GeoJSON.Net.Geometry.LineString(new List<GeoJSON.Net.Geometry.Position>()
                {
                    new GeoJSON.Net.Geometry.Position(XMin, YMin),
                    new GeoJSON.Net.Geometry.Position(XMax, YMin),
                    new GeoJSON.Net.Geometry.Position(XMax, YMax),
                    new GeoJSON.Net.Geometry.Position(XMin, YMax),
                    new GeoJSON.Net.Geometry.Position(XMin, YMin),
                }),
            });
            return polygon;
        }

        public static Extent GetDefault()
        {
            return new Extent { XMin = 5.96F, YMin = 45.82F, XMax = 10.49F, YMax = 47.81F };
        }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
