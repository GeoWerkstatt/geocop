using Microsoft.AspNetCore.Mvc;
using Stac;
using Stac.Api.Interfaces;

namespace GeoCop.Api.StacServices
{
    /// <summary>
    /// Provides access to STAC items.
    /// </summary>
    public class StacItemsBroker : IItemsBroker
    {
        /// <inheritdoc/>
        public Task<StacItem> CreateItemAsync(StacItem stacItem, IStacApiContext stacApiContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task DeleteItemAsync(string featureId, IStacApiContext stacApiContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<StacCollection>> RefreshStacCollectionsAsync(IStacApiContext stacApiContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ActionResult<StacItem>> UpdateItemAsync(StacItem newItem, string featureId, IStacApiContext stacApiContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
