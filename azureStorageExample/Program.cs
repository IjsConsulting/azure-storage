using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace azureStorageExample
{
    class Program
    {
        static CloudStorageAccount storageAccount;
        static CloudBlobClient cloudBlobClient;
        static CloudBlobContainer cloudBlobContainer;
        static CloudBlockBlob cloudBlockBlob;

        static async Task Main()
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                //Create container
                await CreateContainer();
                var sourceFile = await UploadBlob();
                var destinationFile = await DownloadBlob(sourceFile);
                await DeleteBlob(sourceFile, destinationFile);
            }
            else
            {
                // Otherwise, let the user know that they need to define the environment variable.
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add an environment variable named 'AZURE_STORAGE_CONNECTION_STRING' with your storage " +
                    "connection string as a value.");
                Console.WriteLine("Press any key to exit the application.");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Creates a Blob Container.
        /// </summary>
        /// <returns></returns>
        private static async Task CreateContainer() 
        {
            //Create container
            cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Create a container called 'quickstartblobs' and 
            // append a GUID value to it to make the name unique.
            cloudBlobContainer = cloudBlobClient.GetContainerReference("quickstartblobs" +
                    Guid.NewGuid().ToString());

            await cloudBlobContainer.CreateAsync();

            // Set the permissions so the blobs are public.
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await cloudBlobContainer.SetPermissionsAsync(permissions);
        }

        /// <summary>
        /// Uploads a file to Blob storage
        /// </summary>
        /// <returns></returns>
        private static async Task<string> UploadBlob()
        {
            //Update container
            // Create a file in your local MyDocuments folder to upload to a blob.
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string localFileName = "QuickStart_" + Guid.NewGuid().ToString() + ".txt";
            string sourceFile = Path.Combine(localPath, localFileName);

            // Write text to the file.
            File.WriteAllText(sourceFile, "Hello, World!");

            Console.WriteLine("Temp file = {0}", sourceFile);
            Console.WriteLine("Uploading to Blob storage as blob '{0}'", localFileName);

            // Get a reference to the blob address, then upload the file to the blob.
            // Use the value of localFileName for the blob name.
            cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
            await cloudBlockBlob.UploadFromFileAsync(sourceFile);

            //List Blobs
            // List the blobs in the container.
            Console.WriteLine("List blobs in container.");
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    Console.WriteLine(item.Uri);
                }
            } while (blobContinuationToken != null); // Loop while the continuation token is not null.
            
            return sourceFile;
        }

        /// <summary>
        /// Download Blob document
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        private static async Task<string> DownloadBlob(string sourceFile)
        {   
            // Download the blob to a local file, using the reference created earlier.
            // Append the string "_DOWNLOADED" before the .txt extension so that you 
            // can see both files in MyDocuments.
            string destinationFile = sourceFile.Replace(".txt", "_DOWNLOADED.txt");
            Console.WriteLine("Downloading blob to {0}", destinationFile);

            await cloudBlockBlob.DownloadToFileAsync(destinationFile, FileMode.Create);

            return destinationFile;
        }

        /// <summary>
        /// Delete Blob document
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destinationFile"></param>
        /// <returns></returns>
        private static async Task DeleteBlob(string sourceFile, string destinationFile)
        {
            //Delete container.
            Console.WriteLine("Press the 'Enter' key to delete the example files, " +
            "example container, and exit the application.");

            Console.ReadLine();
            
            // Clean up resources. This includes the container and the two temp files.
            Console.WriteLine("Deleting the container");
            if (cloudBlobContainer != null)
            {
                await cloudBlobContainer.DeleteIfExistsAsync();
            }
            
            Console.WriteLine("Deleting the source, and downloaded files");
            File.Delete(sourceFile);
            File.Delete(destinationFile);
        }
    }
}
