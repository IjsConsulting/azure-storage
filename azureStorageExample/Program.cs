using System;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace azureStorageExample
{
    class Program
    {

        static async Task Main()
        {
            //BlobStorage Example
            var blobStorageExample = new BlobStorageExample();
            await blobStorageExample.Run();

            //Queue Example
            var queueStorageExample = new QueueStorageExample();
            await blobStorageExample.Run();
        }

    }
}
