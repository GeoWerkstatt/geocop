using Stac;
using Stac.Api.Interfaces;

namespace GeoCop.Api.StacServices
{
    /// <summary>
    /// Provides access to STAC items.
    /// </summary>
    public class StacItemsProvider : IItemsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StacItemsProvider"/> class.
        /// </summary>
        /// <param name="context"></param>
        public StacItemsProvider(Context context)
        {
            Context = context;
        }

        /// <summary>
        /// Test context.
        /// </summary>
        public Context Context { get; }

        /// <inheritdoc/>
        public bool AnyItemsExist(IEnumerable<StacItem> items, IStacApiContext stacApiContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<StacItem> GetItemByIdAsync(string featureId, IStacApiContext stacApiContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); // return Task.FromResult(Context.Items[featureId]);
        }

        /// <inheritdoc/>
        public string GetItemEtag(string featureId, IStacApiContext stacApiContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<StacItem>> GetItemsAsync(IStacApiContext stacApiContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); // return Task.FromResult<IEnumerable<StacItem>>(Context.Items.Values);
        }
    }
}
