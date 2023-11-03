using Stac;
using System.Net.Mime;

namespace GeoCop.Api.Models
{
    /// <summary>
    /// Represents to a STAC asset.
    /// </summary>
    public class File
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<FileRole> Roles { get; set; }
        public ContentType ContentType { get; set; }

        // public StacAsset ConvertToStacAsset(IStacObject parent)
        // {
        //     return new StacAsset(parent, new Uri(Path), Roles.Select(r => r.ToString()), Name, ContentType);
        // }
    }

    public enum FileRole
    {
        Data,
        Log,
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
