using AzureHF.BlobStorage;
using AzureHF.Models.Index;
using AzureHF.Models.Tree;
using AzureHF.Properties;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace AzureHF.DocumentDB
{
    public class DocumentDBManager
    {

        private static Root rootNode = new Root
        {
            RootNode = new Node()
            {
                Name = "Root",
                Nodes = null,
                NodeId = HiResDateTime.UtcNowTicks.ToString()
            },
            Id = Settings.Default.HierarchyDocument
        };

        public async Task<Document> CreateOrReplaceDocumentAsync(string databaseName, string collectionName, Node root = null)
        {

            if (root != null)
            {
                rootNode.RootNode = root;
            }

            using (var client = new DocumentClient(new Uri(Settings.Default.DocumentDBURI), Settings.Default.DocumentDBPrimaryKey))
            {
                try
                {
                   
                    ResourceResponse<Document> response = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), rootNode);
                    
                    return response.Resource;
                }
                catch (DocumentClientException de)
                {
                    if (de.StatusCode == HttpStatusCode.Conflict)
                    {
                        string documentLink = "/dbs/" + Settings.Default.DocumenDBDatabaseName + "/colls/" + Settings.Default.DocumentDBCollectionName + "/docs/" + Settings.Default.HierarchyDocument;
                        //ResourceResponse<Document> response = await client.ReplaceDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), rootNode);
                        ResourceResponse<Document> response = await client.ReplaceDocumentAsync(documentLink,rootNode);

                        return response.Resource;
                    }
                    else
                    {
                        throw de;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public async Task<Document> GetDocumentAsync(string documentLink)
        {
            using (var client = new DocumentClient(new Uri(Settings.Default.DocumentDBURI), Settings.Default.DocumentDBPrimaryKey))
            {

                try
                {

                    ResourceResponse<Document> response = await client.ReadDocumentAsync(documentLink);

                    return response.Resource;
                }
                catch (DocumentClientException de)
                {
                    if (de.StatusCode == HttpStatusCode.NotFound)
                    {

                        Document response = await CreateOrReplaceDocumentAsync(Settings.Default.DocumenDBDatabaseName,
                            Settings.Default.DocumentDBCollectionName);

                        return response;
                    }
                    throw;
                }

            }
        }


        public List<BlobModel> GetBlobsByNodeId(string databaseName, string collectionName, string nodeId)
        {
            using (var client = new DocumentClient(new Uri(Settings.Default.DocumentDBURI), Settings.Default.DocumentDBPrimaryKey))
            {

                BlobDocument blobDocument;

                try
                {

                    Uri collUri = UriFactory.CreateDocumentCollectionUri(Settings.Default.DocumenDBDatabaseName, Settings.Default.DocumentDBCollectionName);

                    blobDocument = client.CreateDocumentQuery<BlobDocument>(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName)).Where(b => b.NodeId == nodeId).ToList().First();

                    return blobDocument.Blobs;
                }
                catch (InvalidOperationException)
                {
                    return new List<BlobModel>();
                }
            }
        }

        public async Task<Document> AddBlobDocumentAsync(string databaseName, string collectionName, string nodeId, BlobModel newBlob)
        {
            using (var client = new DocumentClient(new Uri(Settings.Default.DocumentDBURI), Settings.Default.DocumentDBPrimaryKey))
            {

                BlobDocument blobDocument;

                try
                {

                    Uri collUri = UriFactory.CreateDocumentCollectionUri(Settings.Default.DocumenDBDatabaseName, Settings.Default.DocumentDBCollectionName);

                    blobDocument = client.CreateDocumentQuery<BlobDocument>(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName)).Where(b => b.NodeId == nodeId).ToList().First();

                    blobDocument.Blobs.Add(newBlob);

                    var response = await client.UpsertDocumentAsync(collUri, blobDocument);

                    return response.Resource;
                }
                catch (InvalidOperationException)
                {

                        blobDocument = new BlobDocument() { NodeId = nodeId, Blobs = new List<BlobModel>() { newBlob } };

                        var response1 = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), blobDocument);

                        return response1.Resource;
                }
                
            }
        }
    }
}