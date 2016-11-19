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
using AzureHF.Models.Index;
using AzureHF.BlobStorage;

namespace AzureHF.Controllers
{
    public class HomeController : Controller
    {

        public static Node root;
        public static Dictionary<string, List<BlobModel>> blobs;


        [Authorize]
        public async Task<ActionResult> Index()
        {
            root = null;
            if (root == null)
            {
                //GetTreeNodes
                var documenDBManager = new DocumentDBManager();

                Document doc = await documenDBManager.GetDocumentAsync("/dbs/" + Settings.Default.DocumenDBDatabaseName + "/colls/" + Settings.Default.DocumentDBCollectionName + "/docs/" + Settings.Default.HierarchyDocument);

                root = doc.GetPropertyValue<Node>("Root");

            }
            blobs = null;
            if (blobs == null)
            {
                //GetBlobs
                //var blobManager = new BlobManager();
                var documenDBManager = new DocumentDBManager();

                blobs = new Dictionary<string, List<BlobModel>>();
                IEnumerable<Node> nodes = root.Descendants();
                foreach (var node in nodes)
                {
                    //blobs.Add(node.NodeId,blobManager.GetBlobInformation(node.Name));
                    blobs.Add(node.NodeId, documenDBManager.GetBlobsByNodeId(Settings.Default.DocumenDBDatabaseName,
                            Settings.Default.DocumentDBCollectionName,node.NodeId));
                }
            }

            IndexModel model = new IndexModel();
            model.Root = root;
            model.Blobs = blobs;

            return View(model);
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


            HomeController.root = root;

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


            HomeController.root = root;

            return RedirectToAction("Index");
        }
       
        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file, string nodeId)
        {

            var containerNode = root.Descendants().Where(n => n.NodeId == nodeId).First();
            var virtualContainer = containerNode.Name + "_(" + containerNode.NodeId + ")" +"/";
            var collectionName = virtualContainer + file.FileName;
            var blobModel = new BlobModel() { DisplayName = file.FileName, Path = collectionName};

            //UploadBlob
            var blobManager = new BlobManager();
            blobManager.UploadBlob(file, virtualContainer);

            //Update documentDB
            var documentDBManager = new DocumentDBManager();

            await documentDBManager.AddBlobDocumentAsync(Settings.Default.DocumenDBDatabaseName, Settings.Default.DocumentDBCollectionName, nodeId, blobModel);

            //Add blobname to list
            List<BlobModel> blobList;

            if (blobs.TryGetValue(nodeId, out blobList))
            {
                blobList.Add(blobModel);
            }
            else
            {
                blobList = new List<BlobModel>();
                blobList.Add(blobModel);
            }

            blobs[nodeId] = blobList;



            return RedirectToAction("Index");
        }


        private Node DeleteNode(string nodeId, string parentNodeId)
        {

            root.Descendants().Where(n => n.NodeId == parentNodeId).First().SafeNodes.RemoveByNodeId(nodeId);

            return root;
        }

        private Node AddNode(string name, string parent)
        {

            Node newNode = new Node() { Name = name, Nodes = null, NodeId = HiResDateTime.UtcNowTicks.ToString() };

            root.Descendants().Where(n => n.NodeId == parent).First().SafeNodes.Add(newNode);

            return root;
        }


        public ActionResult DownloadFile(string name)
        {

            BlobManager blobManager = new BlobManager();

            Response.AddHeader("Content-Disposition", "attachment; filename=" + name); 

            blobManager.GetBlob(name).DownloadToStream(Response.OutputStream);

            return new EmptyResult();
        }

        
        [HttpDelete]
        public async Task<ActionResult> DeleteFile(string nodeId, string name)
        {
            //Delete Blob
            BlobManager blobManager = new BlobManager();

            blobManager.DeleteBlob(name);


            //Delete from documentDB
            DocumentDBManager documentDB = new DocumentDBManager();
            await documentDB.RemoveBlobDocumentAsync(Settings.Default.DocumenDBDatabaseName, Settings.Default.DocumentDBCollectionName, nodeId, name);

            //Delet from list
            List<BlobModel> blobList;

            if (blobs.TryGetValue(nodeId, out blobList))
            {
                int index = blobList.FindIndex(b => b.Path == name);
                blobList.RemoveAt(index);
                blobs[nodeId] = blobList;
            }

            return RedirectToAction("Index");
        }

    }

}