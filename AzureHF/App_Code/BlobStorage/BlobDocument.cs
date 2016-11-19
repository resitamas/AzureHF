using AzureHF.Models.Index;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureHF.BlobStorage
{
    public class BlobDocument : Resource
    {

        [JsonProperty("NodeId")]
        public string NodeId { get; set; }

        [JsonProperty("Blobs")]
        public List<BlobModel> Blobs { get; set; }

    }
}