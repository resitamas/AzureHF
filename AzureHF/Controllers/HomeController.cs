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
        //[Authorize]
        public async Task<ActionResult> Index()
        {

            var documenDBManager = new DocumentDBManager();

            Document doc = await documenDBManager.GetDocumentAsync("/dbs/" + Settings.Default.DocumenDBDatabaseName + "/colls/" + Settings.Default.DocumentDBCollectionName + "/docs/" + Settings.Default.HierarchyDocument);


            Node root = doc.GetPropertyValue<Node>("Root");

            Session["Node"] = root;

            //root.Nodes = new List<Node>();
            //root.Nodes.Add(new Node() { Name = "Child1" });
            //root.Nodes.Add(new Node() { Name = "Child2", Nodes = new List<Node>() { new Node() { Name = "GrandChild1"} } });
            //root.Nodes.Add(new Node() { Name = "Child3" });

            return View(root);
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
        public ActionResult AddDirectory(string name, string parent)
        {

            if (parent == "")
                return RedirectToAction("Index");


            return RedirectToAction("Index");
        }

    }

}