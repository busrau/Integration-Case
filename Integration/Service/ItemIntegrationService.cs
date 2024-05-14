using System.Collections.Concurrent;
using System.Threading;
using Integration.Common;
using Integration.Backend;

namespace Integration.Service
{
    public sealed class ItemIntegrationService
    {
        private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> locks = new();

        public Result SaveItem(string itemContent)
        {
            var semaphore = locks.GetOrAdd(itemContent, _ => new SemaphoreSlim(1, 1));

            semaphore.Wait();
            try
            {
                if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
                {
                    return new Result(false, $"Duplicate item received with content {itemContent}.");
                }

                var item = ItemIntegrationBackend.SaveItem(itemContent);
                return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public List<Item> GetAllItems()
        {
            return ItemIntegrationBackend.GetAllItems();
        }
    }
}
