using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WePrint.Utilities
{
    public interface IBlobContainerProvider
    {
        CloudBlobContainer GetContainerReference(string containerName);
    }

    public class BlobContainerProvider : IBlobContainerProvider
    {
        private readonly IConfiguration _configuration;

        public BlobContainerProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CloudBlobContainer GetContainerReference(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureStorage"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }
    }
}
