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
                    DocumentCollection col = new DocumentCollection();
                    
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



    }
}