using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Queue;

namespace azureStorageExample
{
    public class QueueStorageExample : IStorageAccount
    {
        string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");


        public async Task Run()
        {
            //Create queue container
            await CreateQueueAsync();

            await InsertMessageIntoQueueAsync();

            await PeekQueueMessagesAsync();

            await GetMessageFromQueueAsync();
        }

        /// <summary>
        /// Create MQ
        /// </summary>
        /// <returns></returns>
        private async Task CreateQueueAsync()
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Insert a message into the MQ.
        /// </summary>
        /// <returns></returns>
        private async Task InsertMessageIntoQueueAsync()
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();

            // Create a message and add it to the queue
            CloudQueueMessage message = new CloudQueueMessage("Hello, World");
            await queue.AddMessageAsync(message);
        }

        /// <summary>
        /// Peek a message on the MQ
        /// </summary>
        /// <returns></returns>
        private async Task PeekQueueMessagesAsync()
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Peek at the next message
            CloudQueueMessage peekedMessage = await queue.PeekMessageAsync();

            // Display message.
            Console.WriteLine(peekedMessage.AsString);
        }

        /// <summary>
        /// Get a message from the MQ
        /// </summary>
        /// <returns></returns>
        private async Task GetMessageFromQueueAsync()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Get the message from the queue and update the message contents.
            CloudQueueMessage message = await queue.GetMessageAsync();
            message.SetMessageContent2("Updated contents.", false);
            queue.UpdateMessage(message,
                TimeSpan.FromSeconds(60.0),  // Make it invisible for another 60 seconds.
                MessageUpdateFields.Content | MessageUpdateFields.Visibility);
        }

        /// <summary>
        /// Delete a message from MQ
        /// </summary>
        /// <returns></returns>
        private async Task DeleteMessage()
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Get the next message
            CloudQueueMessage retrievedMessage = await queue.GetMessageAsync();

            //Process the message in less than 30 seconds, and then delete the message
            await queue.DeleteMessageAsync(retrievedMessage);
        }
    }
}
