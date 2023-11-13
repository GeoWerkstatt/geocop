﻿using Microsoft.EntityFrameworkCore;
using Stac;
using Stac.Api.Interfaces;

namespace GeoCop.Api.StacServices
{
    /// <summary>
    /// Provides access to STAC items.
    /// </summary>
    public class StacItemsProvider : IItemsProvider
    {
        private readonly ILogger<StacItemsProvider> logger;
        private readonly IDbContextFactory<Context> contextFactory;
        private readonly StacConverter stacConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="StacItemsProvider"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="contextFactory"></param>
        /// <param name="stacConverter"></param>
        public StacItemsProvider(ILogger<StacItemsProvider> logger, IDbContextFactory<Context> contextFactory, StacConverter stacConverter)
        {
            this.logger = logger;
            this.contextFactory = contextFactory;
            this.stacConverter = stacConverter;
        }

        /// <inheritdoc/>
        public bool AnyItemsExist(IEnumerable<StacItem> items, IStacApiContext stacApiContext)
        {
            using var db = contextFactory.CreateDbContext();
            foreach (var collection in stacApiContext.Collections)
            {
                try
                {
                    if (db.DeliveryMandatesWithIncludes.First(dm => (StacConverter.CollectionIdPrefix + dm.Id) == collection).Deliveries.Any())
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
                using var db = contextFactory.CreateDbContext();
                var delivery = db.DeliveriesWithIncludes
                    .FirstOrDefault(d => (StacConverter.ItemIdPrefix + d.Id) == featureId && (StacConverter.CollectionIdPrefix + d.DeliveryMandate.Id == stacApiContext.Collections.First()))
                    ?? throw new InvalidOperationException($"Item with id {featureId} does not exist.");
                var item = stacConverter.ToStacItem(delivery);
                return Task.FromResult(item);
            }
            catch (Exception ex)
            {
                var message = $"Error while getting item with id {featureId}.";
                logger.LogError(ex, message);
                throw new InvalidOperationException(message, ex);
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
            using var db = contextFactory.CreateDbContext();
            var deliveryMandates = db.DeliveryMandatesWithIncludes;
            if (!(collectionIds == null || !collectionIds.Any()))
            {
                deliveryMandates = deliveryMandates.Where(dm => collectionIds.Contains(StacConverter.CollectionIdPrefix + dm.Id));
            }

            deliveryMandates.ToList().ForEach(dm => items.Concat(dm.Deliveries.Select(d => stacConverter.ToStacItem(d))));
            return Task.FromResult(items);
        }
    }
}