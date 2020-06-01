using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace azureStorageExample
{
    /// <summary>
    /// Examples uses v12 libraries
    /// </summary>
    public class BlobStorageExample : IStorageAccount
    {
        string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

        BlobContainerClient containerClient;
        BlobClient blobClient;

        public async Task Run()
        {
            //Create container
            await CreateContainerAsync();

            //Update a document to the blob container
            string localFilePath = await UploadBlobAsync();

            //Show the contents of the container
            await ListBlobsAsync();

            //Download the document locally
            var downloadFilePath = await DownloadBlobAsync(localFilePath, blobClient);
           
            //Clean up
            await DeleteBlobAsync(localFilePath, downloadFilePath);
        }

        /// <summary>
        /// Creates a Blob Container.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            //Create a unique name for the container
            string containerName = "quickstartblobs" + Guid.NewGuid().ToString();

            // Create the container and return a container client object
            containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
        }

        /// <summary>
        /// Uploads a file to Blob storage
        /// </summary>
        /// <returns></returns>
        private async Task<string> UploadBlobAsync()
        {
            // Create a local file in the ./data/ directory for uploading and downloading
            string localPath = "./data/";
            string fileName = "quickstart" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);

            // Write text to the file
            await File.WriteAllTextAsync(localFilePath, "Hello, World!");

            // Get a reference to a blob
            blobClient = containerClient.GetBlobClient(fileName);

            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            // Open the file and upload its data
            using FileStream uploadFileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();

            return localFilePath;
        }

        /// <summary>
        /// List blobs
        /// </summary>
        /// <returns></returns>
        private async Task ListBlobsAsync()
        {
            Console.WriteLine("Listing blobs...");

            // List all blobs in the container
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }
        }

        /// <summary>
        /// Download Blob document
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        private async Task<string> DownloadBlobAsync(string localFilePath, BlobClient blobClient)
        {
            // Download the blob to a local file
            // Append the string "DOWNLOAD" before the .txt extension 
            // so you can compare the files in the data directory
            string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOAD.txt");

            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                await download.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
            }

            return downloadFilePath;
        }

        /// <summary>
        /// Delete Blob document
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="downloadFilePath"></param>
        /// <returns></returns>
        private async Task DeleteBlobAsync(string localFilePath, string downloadFilePath)
        {
            // Clean up
            Console.Write("Press any key to begin clean up");
            Console.ReadLine();

            Console.WriteLine("Deleting blob container...");
            await containerClient.DeleteAsync();

            Console.WriteLine("Deleting the local source and downloaded files...");
            File.Delete(localFilePath);
            File.Delete(downloadFilePath);

            Console.WriteLine("Done");
        }
    }
}