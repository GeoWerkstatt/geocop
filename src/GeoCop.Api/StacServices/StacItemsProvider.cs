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
            foreach (var collection in stacApiContext.Collections)
            {
                try
                {
                    if (Context.Collections[collection].GetItemLinks().Any())
                    {
                        return true;
                    }
                }
                catch (IOException)
                {
                    return false;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public Task<StacItem> GetItemByIdAsync(string featureId, IStacApiContext stacApiContext, CancellationToken cancellationToken)
        {
            try
            {
                var collection = Context.Collections[stacApiContext.Collections.First()];
                var link = (StacObjectLink)collection.GetItemLinks().Where(link => link.Uri.ToString().Split("/").Last() == featureId).First();
                return Task.FromResult((StacItem)link.StacObject);
            }
            catch (IOException)
            {
                return Task.FromResult<StacItem>(null);
            }
        }

        /// <inheritdoc/>
        public string GetItemEtag(string featureId, IStacApiContext stacApiContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<StacItem>> GetItemsAsync(IStacApiContext stacApiContext, CancellationToken cancellationToken)
        {
            IEnumerable<StacItem> items = new List<StacItem>();

            var collectionIds = stacApiContext.Collections?.ToList();
            if (collectionIds == null || !collectionIds.Any())
            {
                Context.Collections.Values.ToList().ForEach(c =>
                {
                    items = items.Concat(c.GetItemLinks().Select(l => (StacItem)((StacObjectLink)l).StacObject));
                });
            }
            else
            {
                collectionIds.ForEach(collectionId =>
                {
                    items = items.Concat(Context.Collections[collectionId].GetItemLinks().Select(l => (StacItem)((StacObjectLink)l).StacObject));
                });
            }

            return Task.FromResult(items);
        }
    }
}
