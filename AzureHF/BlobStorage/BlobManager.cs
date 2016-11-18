using AzureHF.Models.Index;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureHF.BlobStorage
{
    public class BlobManager
    {

        CloudStorageAccount storageAccount;
        CloudBlobClient blobClient;
        CloudBlobContainer container;


        public BlobManager(string containerName = "azurehf")
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            blobClient = storageAccount.CreateCloudBlobClient();

            container = blobClient.GetContainerReference(containerName);

        }

        public void UploadBlob(HttpPostedFileBase file, string virtualContainer)
        {
            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(virtualContainer+file.FileName);

            using (var fileStream = file.InputStream)
            {
                blockBlob.UploadFromStream(fileStream);
            }

        }


        public List<BlobModel> GetBlobInformation(string virtualDirectory)
        {
            var model = new List<BlobModel>();
            try
            {
                
                foreach (var blob in container.ListBlobs(virtualDirectory, true, BlobListingDetails.All).Cast<CloudBlockBlob>())
                {
                    model.Add(new BlobModel() { DisplayName = blob.Uri.Segments.Last().ToString(), Path = blob.Uri.ToString() });
                }

                return model;

            }
            catch (Exception e)
            {

                return model;
            }
            
        }

    }
}