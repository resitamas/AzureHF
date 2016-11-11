using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureHF.Models.Tree
{
    public class Root
    {

        [JsonProperty("Root")]
        public Node RootNode { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

    }
}