using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureHF.Models.Index
{
    public class BlobModel
    {

        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        
    }
}