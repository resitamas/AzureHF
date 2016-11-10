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

        public async Task<Document> CreateOrReplaceDocumentAsync(string databaseName, string collectionName, string document)
        {
            using (var client = new DocumentClient(new Uri(Settings.Default.DocumentDBURI), Settings.Default.DocumentDBPrimaryKey))
            {
                try
                {
                    return  await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), document);

                    //await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, family.Id));
                }
                catch (DocumentClientException de)
                {
                    if (de.StatusCode == HttpStatusCode.Conflict)
                    {
                        return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), document);
                    }
                    else
                    {
                        throw de;
                    }
                }
            }
        }

        public async Task<Document> GetDocumentAsync(string documentLink)
        {
            using (var client = new DocumentClient(new Uri(Settings.Default.DocumentDBURI), Settings.Default.DocumentDBPrimaryKey))
            {

                try
                {
                    return await client.ReadDocumentAsync(documentLink);
                }
                catch (DocumentClientException de)
                {
                    if (de.StatusCode == HttpStatusCode.NotFound)
                    {
                        return await CreateOrReplaceDocumentAsync(Settings.Default.DocumenDBDatabaseName,
                            Settings.Default.DocumentDBCollectionName,
                            Settings.Default.HierarchyDocument);
                    }
                    throw;
                }

            }
        }



    }
}