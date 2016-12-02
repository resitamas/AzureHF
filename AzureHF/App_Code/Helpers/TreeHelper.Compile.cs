using AzureHF.Models.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureHF.App_Code.Helpers
{
	public static partial class TreeHelper
	{
        public static IEnumerable<Node> Descendants(this Node root)
        {
            var nodes = new Stack<Node>(new[] { root });
            while (nodes.Any())
            {
                Node node = nodes.Pop();
                yield return node;
                if (node.Nodes != null)
                {
                    foreach (var n in node.Nodes) nodes.Push(n);
                }
            }
        }

        public static void RemoveByNodeId(this IList<Node> nodes, string nodeId)
        {
            foreach (var node in nodes)
            {
                if (node.NodeId == nodeId)
                {
                    nodes.Remove(node);
                    return;
                }
            }
        }

    }
}