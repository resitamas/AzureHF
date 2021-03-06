﻿using System.Web;
using System.Web.Optimization;

namespace AzureHF
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/jsTreeCSS").IncludeDirectory(
                     "~/Content/jsTree", "*.css", true
                     ));

            //bundles.Add(new StyleBundle("~/bundles/jsTreeCSS").Include(
            //         "~/Content/jsTree/themes/default/*.css"
            //         ));

            bundles.Add(new ScriptBundle("~/bundles/jstreeJS").IncludeDirectory(
                     "~/Scripts/jsTree3","*.js",true
                     ));

            bundles.Add(new StyleBundle("~/bundles/jqueryuiCSS").IncludeDirectory(
                     "~/Content/Themes", "*.css", true
                     ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                     "~/Scripts/jquery-ui-{version}.js"
                     ));
        }
    }
}
