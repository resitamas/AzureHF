using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureHF.Models.Tree
{
    public class Node
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public List<Node> Nodes { get; set; }

    }
}