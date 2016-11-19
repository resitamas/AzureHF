using AzureHF.Models.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureHF.Helpers
{
    public static class TreeHelper
    {

        public static MvcHtmlString CreateTree(this HtmlHelper helper, Node node)
        {
            string htmlString = "<ul>";

            htmlString = AppendChildNode(htmlString, node);

            return MvcHtmlString.Create(htmlString + "</ul>");
        }

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

        public static string CreateList(this HtmlHelper helper, string nodeId)
        {
            string html = "";




            return html;
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

        private static string AppendChildNode(string html, Node node)
        {

            html += "<li data-nodeid= "+  node.NodeId +" >";

            html += node.Name;

            if (node.Nodes != null && node.Nodes.Count > 0)
            {
                html += "<ul>";

                foreach (var n in node.Nodes)
                {
                    html = AppendChildNode(html, n);
                }
                html += "</ul>";
            }

            html += "</li>";

            return html;
        }
    }
}