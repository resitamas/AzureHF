using AzureHF.Models.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureHF.Models.Index
{
    public class IndexModel
    {

        public Node Root { get; set; }

        public Dictionary<string,List<BlobModel>> Blobs { get; set; }

    }
}