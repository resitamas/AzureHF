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
                Nodes = null
            },
            Id = Settings.Default.HierarchyDocument
        };

        public async Task<Document> CreateOrReplaceDocumentAsync(string databaseName, string collectionName)
        {
            using (var client = new DocumentClient(new Uri(Settings.Default.DocumentDBURI), Settings.Default.DocumentDBPrimaryKey))
            {
                try
                {
                   
                    ResourceResponse<Document> response = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), rootNode);
                    DocumentCollection col = new DocumentCollection();
                    
                    return response.Resource;

                    //await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, family.Id));
                }
                catch (DocumentClientException de)
                {
                    if (de.StatusCode == HttpStatusCode.Conflict)
                    {

                        ResourceResponse<Document> response = await client.ReplaceDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), rootNode);

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