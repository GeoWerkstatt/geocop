using Stac;
using Stac.Api.Interfaces;
using Stac.Collection;
using System.Globalization;

namespace GeoCop.Api.StacServices
{
    /// <summary>
    /// Provides access to STAC collections.
    /// </summary>
    public class StacCollectionsProvider : ICollectionsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StacCollectionsProvider"/> class.
        /// </summary>
        /// <param name="context"></param>
        public StacCollectionsProvider(Context context)
        {
            Context = context;
        }

        /// <summary>
        /// Test context.
        /// </summary>
        public Context Context { get; }

        /// <inheritdoc/>
        public Task<StacCollection> GetCollectionByIdAsync(string collectionId, IStacApiContext stacApiContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Context.Collections[collectionId]);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<StacCollection>> GetCollectionsAsync(IStacApiContext stacApiContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<StacCollection>>(Context.Collections.Values);
        }
    }
}
