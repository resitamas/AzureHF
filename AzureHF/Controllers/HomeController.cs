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
        public async Task<ActionResult> AddDirectory(string name, string parent)
        {

            if (parent == "")
                return RedirectToAction("Index");


            Node root = AddNode(name,parent);


            var documenDBManager = new DocumentDBManager();

            Document doc = await documenDBManager.CreateOrReplaceDocumentAsync(Settings.Default.DocumenDBDatabaseName,
                            Settings.Default.DocumentDBCollectionName,root);


            node = root;

            return RedirectToAction("Index");
        }

        private Node AddNode(string name, string parent)
        {

            Node root = node;

            ChechId(parent, root).Nodes.Add(new Node() { Name = name, Nodes = null, NodeId = HiResDateTime.UtcNowTicks.ToString()});

            return root;

        }

        private Node ChechId(string parent, Node node)
        {
            
            if (node.NodeId == parent)
            {
                if (node.Nodes == null)
                {
                    node.Nodes = new List<Node>();
                }

                return node;
            }

            if (node.Nodes != null)
            {
                foreach (var n in node.Nodes)
                {
                    return ChechId(n.NodeId, n);
                }
            }

            return null;

        }
    }

}