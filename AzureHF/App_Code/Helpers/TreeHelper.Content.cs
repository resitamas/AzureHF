using AzureHF.Models.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureHF.App_Code.Helpers
{
    public static partial class TreeHelper
    {

        public static MvcHtmlString CreateTree(this HtmlHelper helper, Node node)
        {
            string htmlString = "<ul>";

            htmlString = AppendChildNode(htmlString, node);

            return MvcHtmlString.Create(htmlString + "</ul>");
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