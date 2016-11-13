using AzureHF.DocumentDB;
using AzureHF.Models.Tree;
using AzureHF.Properties;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AzureHF.Helpers;

namespace AzureHF.Controllers
{
    public class HomeController : Controller
    {

        public static Node node;

        //[Authorize]
        public async Task<ActionResult> Index()
        {

            if (node == null)
            {
                var documenDBManager = new DocumentDBManager();

                Document doc = await documenDBManager.GetDocumentAsync("/dbs/" + Settings.Default.DocumenDBDatabaseName + "/colls/" + Settings.Default.DocumentDBCollectionName + "/docs/" + Settings.Default.HierarchyDocument);

                Node root = doc.GetPropertyValue<Node>("Root");
                node = root;
                ViewData["Node"] = root;
            }
            else
            {
                ViewData["Node"] = node;
            }

            //root.Nodes = new List<Node>();
            //root.Nodes.Add(new Node() { Name = "Child1" });
            //root.Nodes.Add(new Node() { Name = "Child2", Nodes = new List<Node>() { new Node() { Name = "GrandChild1"} } });
            //root.Nodes.Add(new Node() { Name = "Child3" });

            return View(ViewData["Node"]);
        }

        //[AzureADAuthorizedAttribute(Role = Authorization.Role.Reader)]
        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //[Authorize(Roles = "Reader")]
        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        [HttpPost]
        public async Task<ActionResult> AddDirectory(string name, string parentNodeId)
        {

            if (parentNodeId == "")
                return RedirectToAction("Index");


            Node root = AddNode(name,parentNodeId);


            var documenDBManager = new DocumentDBManager();

            Document doc = await documenDBManager.CreateOrReplaceDocumentAsync(Settings.Default.DocumenDBDatabaseName,
                            Settings.Default.DocumentDBCollectionName,root);


            node = root;

            return RedirectToAction("Index");
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteDirectory(string nodeId, string parentNodeId)
        {

            if (nodeId == "" || parentNodeId == "" || nodeId == "undwefined" || parentNodeId == "undefined")
            {
                return RedirectToAction("Index");
            }


            Node root = DeleteNode(nodeId, parentNodeId);


            var documenDBManager = new DocumentDBManager();

            Document doc = await documenDBManager.CreateOrReplaceDocumentAsync(Settings.Default.DocumenDBDatabaseName,
                            Settings.Default.DocumentDBCollectionName, root);


            node = root;

            return RedirectToAction("Index");
        }

        private Node DeleteNode(string nodeId, string parentNodeId)
        {

            Node root = node;

            root.Descendants().Where(n => n.NodeId == parentNodeId).First().SafeNodes.RemoveByNodeId(nodeId);

            //List<Node> nodes = CheckId(nodeId, root).Nodes;

            //nodes.Remove(nodes.Find(n => n.NodeId == nodeId));

            return root;
        }

        private Node AddNode(string name, string parent)
        {

            Node root = node;

            Node newNode = new Node() { Name = name, Nodes = null, NodeId = HiResDateTime.UtcNowTicks.ToString() };

            root.Descendants().Where(n => n.NodeId == parent).First().SafeNodes.Add(newNode);

            return root;
        }
    }

}