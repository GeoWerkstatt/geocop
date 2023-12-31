﻿using Stac;
using Stac.Api.Interfaces;

namespace GeoCop.Api.StacServices;

/// <summary>
/// Provides access to STAC root catalog.
/// </summary>
public class StacRootCatalogProvider : IRootCatalogProvider
{
    /// <inheritdoc/>
    public Task<StacCatalog> GetRootCatalogAsync(IStacApiContext stacApiContext, CancellationToken cancellationToken = default)
    {
        var catalog = new StacCatalog("geocop", "Root catalog for geocop");
        return Task.FromResult(catalog);
    }
}
