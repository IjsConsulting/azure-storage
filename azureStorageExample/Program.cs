using System.Threading.Tasks;

namespace azureStorageExample
{
    class Program
    {
        static async Task Main()
        {
            //BlobStorage Example
            var blobStorageExample = new BlobStorageExample();
            await blobStorageExample.Run();

            //QueueStorage Example
            var queueStorageExample = new QueueStorageExample();
            await queueStorageExample.Run();
        }
    }
}
