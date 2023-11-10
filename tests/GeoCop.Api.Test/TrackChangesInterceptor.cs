using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Concurrent;

namespace GeoCop.Api.Test
{
    /// <summary>
    /// Keep track of which objects we need to clear for the next test run.
    /// </summary>
    public class TrackChangesInterceptor : ISaveChangesInterceptor
    {
        private readonly ConcurrentBag<object> addedEntities = new ConcurrentBag<object>();
        private readonly ConcurrentDictionary<object, PropertyValues?> modifiedEntities = new ConcurrentDictionary<object, PropertyValues?>();
        private readonly ConcurrentBag<object> deletedEntities = new ConcurrentBag<object>();

        public void SaveChangesFailed(DbContextErrorEventData eventData) { }
        public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public int SavedChanges(SaveChangesCompletedEventData eventData, int result) => result;
        public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default) => new ValueTask<int>(result);

        public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            LogModifications(eventData.Context);
            return result;
        }

        public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            LogModifications(eventData.Context);
            return result;
        }

        private void LogModifications(DbContext context)
        {
            context.ChangeTracker.DetectChanges();

            foreach (EntityEntry entry in context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        addedEntities.Add(entry.Entity);
                        break;
                    case EntityState.Modified:
                        if (!addedEntities.Contains(entry.Entity))
                        {
                            modifiedEntities.TryAdd(entry.Entity, entry.GetDatabaseValues());
                        }

                        break;
                    case EntityState.Deleted:
                        if (!addedEntities.Contains(entry.Entity))
                        {
                            deletedEntities.Add(entry.Entity);
                        }

                        break;
                    case EntityState.Unchanged:
                    default:
                        continue;
                }
            }
        }

        public void ResetContext(DbContext context)
        {
            context.ChangeTracker.Clear();
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            foreach (object entity in addedEntities)
            {
                context.Entry(entity).State = EntityState.Deleted;
            }

            foreach (object entity in deletedEntities)
            {
                context.Entry(entity).State = EntityState.Added;
            }

            foreach (KeyValuePair<object, PropertyValues?> entity in modifiedEntities)
            {
                if (entity.Value is null)
                {
                    context.Entry(entity.Key).State = EntityState.Deleted;
                }
                else
                {
                    context.Entry(entity.Key).CurrentValues.SetValues(entity.Value);
                }
            }

            addedEntities.Clear();
            deletedEntities.Clear();
            modifiedEntities.Clear();

            context.ChangeTracker.DetectChanges();
            context.SaveChanges();
        }
    }
}
