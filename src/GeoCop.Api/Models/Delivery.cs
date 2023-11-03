using GeoJSON.Net.Geometry;
using Itenso.TimePeriod;
using Stac;
using Stac.Api.Interfaces;
using Stac.Api.WebApi.Services;
using System.ComponentModel.DataAnnotations;

namespace GeoCop.Api.Models
{
    /// <summary>
    /// Represents to a STAC item.
    /// </summary>
    public class Delivery
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Operat? Operat { get; set; }
        public IGeometryObject? Geometry { get; set; }
        public DateTime UploadDate { get; set; }
        public List<File> Files { get; set; }

        // private IStacLinker Linker { get; }
        // private IStacApiContext StacApiContext { get; }

        // public StacItem ConvertToStacItem(string collectionId, IGeometryObject geometry)
        // {
        //     var stacId = Name + "_" + Id;

        //     var item = new StacItem(stacId, geometry)
        //     {
        //         Collection = collectionId,
        //         Description = Description,
        //     };

        //     Linker.Link(item, StacApiContext);

        //     item.DateTime = new TimePeriodChain();
        //     item.DateTime.Setup(UploadDate, UploadDate);

        //     var assets = Files.Select(f => f.ConvertToStacAsset(item)).ToDictionary(a => a.Title);
        //     item.Assets.AddRange(assets);
        //     return item;
        // }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
