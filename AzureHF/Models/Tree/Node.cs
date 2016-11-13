using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureHF.Models.Tree
{
    public class Node
    {

        public string NodeId { get; set; }

        public string Name { get; set; }

        public List<Node> Nodes { get; set; }

        [JsonIgnore]
        public List<Node> SafeNodes
        {
            get
            {
                if (Nodes == null)
                {
                    Nodes = new List<Node>();
                }

                return Nodes;
            }
        }


    }
}