using Integration.Common;
using Integration.DistributedService;
using Integration.Service;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Integration
{
    public abstract class Program
    {
        public static async Task Main(string[] args)
        {
            var service = new ItemIntegrationService();
            var distributedService = new DistributedItemIntegrationService(service);

            var tasks = new List<Task>();
            tasks.Add(Task.Run(() => LogResult(service.SaveItem("a"))));
            tasks.Add(Task.Run(() => LogResult(service.SaveItem("b"))));
            tasks.Add(Task.Run(() => LogResult(service.SaveItem("c"))));

            Thread.Sleep(500);

            tasks.Add(Task.Run(() => LogResult(service.SaveItem("a"))));
            tasks.Add(Task.Run(() => LogResult(service.SaveItem("b"))));
            tasks.Add(Task.Run(() => LogResult(service.SaveItem("c"))));

            tasks.Add(Task.Run(async () => LogResult(await distributedService.SaveItemAsync("d"))));
            tasks.Add(Task.Run(async () => LogResult(await distributedService.SaveItemAsync("e"))));
            tasks.Add(Task.Run(async () => LogResult(await distributedService.SaveItemAsync("f"))));

            await Task.Delay(500);

            tasks.Add(Task.Run(async () => LogResult(await distributedService.SaveItemAsync("d"))));
            tasks.Add(Task.Run(async () => LogResult(await distributedService.SaveItemAsync("e"))));
            tasks.Add(Task.Run(async () => LogResult(await distributedService.SaveItemAsync("f"))));

            await Task.WhenAll(tasks);
            Console.WriteLine("Everything recorded:");

            service.GetAllItems().ForEach(item => Console.WriteLine($"Item ID: {item.Id}, Content: {item.Content}"));

            Console.ReadLine();
        }

        private static void LogResult(Result result)
        {
            Console.WriteLine(result.Message);
        }
    }
}
