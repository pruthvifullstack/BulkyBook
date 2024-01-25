using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace BulkyBookWeb.Services
{
    public class AzureStorageService
    {
        private readonly IConfiguration _configuration;

        public AzureStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadBlobAsync(IFormFile file, string containerName)
        {
            string storageAccountConnectionString = _configuration.GetSection("AzureStorage:ConnectionString").Value;

            // Get a reference to the Azure Storage Emulator
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);

            // Create the blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Create a container (you may need to create this container manually)
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Ensure the container exists
            await container.CreateIfNotExistsAsync();

            // Create a unique name for the blob
            string blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // Get a reference to the blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            // Upload the file to the blob
            using (Stream stream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(stream);
            }

            // Return the URL of the uploaded blob
            return blockBlob.Uri.ToString();
        }
    }
}
