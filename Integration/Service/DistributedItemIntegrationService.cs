using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Integration.Common;
using Integration.Service;

namespace Integration.DistributedService
{
    public class DistributedItemIntegrationService
    {
        private readonly ItemIntegrationService _itemIntegrationService;
        private static readonly ConnectionMultiplexer RedisConnection = ConnectionMultiplexer.Connect("localhost");
        private static readonly IDatabase RedisDb = RedisConnection.GetDatabase();

        public DistributedItemIntegrationService(ItemIntegrationService itemIntegrationService)
        {
            _itemIntegrationService = itemIntegrationService;
        }

        public async Task<Result> SaveItemAsync(string itemContent)
        {
            var lockKey = $"lock:item:{itemContent}";
            var lockToken = Guid.NewGuid().ToString();
            var isLockAcquired = await RedisDb.LockTakeAsync(lockKey, lockToken, TimeSpan.FromSeconds(30));

            if (!isLockAcquired)
            {
                return new Result(false, $"Could not acquire lock for content {itemContent}. Try again later.");
            }

            try
            {
                return _itemIntegrationService.SaveItem(itemContent);
            }
            finally
            {
                await RedisDb.LockReleaseAsync(lockKey, lockToken);
            }
        }

        public List<Item> GetAllItems()
        {
            return _itemIntegrationService.GetAllItems();
        }
    }
}
