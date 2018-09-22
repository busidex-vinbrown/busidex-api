using System.Web.Optimization;

namespace BootstrapSupport
{
    public class BootstrapBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.FileSetOrderList.Clear(); 

            bundles.Add(new ScriptBundle("~/Scripts/js").Include(
                            "~/Scripts/jquery-1.9.1.js",
                            "~/Scripts/jquery-ui-1.10.0.js",
                "~/Scripts/jquery-migrate-1.1.1.js"

            ));
            bundles.Add(new ScriptBundle("~/Scripts/js2").Include(

                "~/Scripts/AjaxLogin.js",
                "~/Scripts/modernizr-2.6.2.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.form.js",
                "~/Scripts/Busidex.js",
                "~/Scripts/Home.js"
                            ));

            bundles.Add(new ScriptBundle("~/Scripts/registration").Include(
                "~/Scripts/Registration.js"
                            ));
            bundles.Add(new ScriptBundle("~/Scripts/search").Include(
                "~/Scripts/search.js"
                            ));
            bundles.Add(new ScriptBundle("~/Scripts/addcard").Include(
                "~/Scripts/AddCard.js"
                            ));
            bundles.Add(new ScriptBundle("~/Scripts/mybusidex").Include(
                "~/Scripts/MyBusidex.js"
                            ));
            bundles.Add(new ScriptBundle("~/Scripts/mygroups").Include(
                "~/Scripts/MyBusidex.js",
                "~/Scripts/groups.js"
                            ));
            #region styles

            var bootstrapStyles = new StyleBundle("~/content/css");
            bootstrapStyles.Include(
                "~/Content/reset-min.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-responsive.css",
                "~/Content/body.css",
                "~/Content/bootstrap-mvc-validation.css",
                "~/Content/themes/base/jquery-ui.css"
                );
            bundles.Add(bootstrapStyles);

            var busidexStyles = new StyleBundle("~/Content/busidex");
            busidexStyles.Include(
                "~/Content/Busidex.css",
                "~/Content/Site.css"
                );

            bundles.Add(busidexStyles);
            bundles.Add(new StyleBundle("~/Content/search").Include(
                "~/Content/Search.css"
            ));
            bundles.Add(new StyleBundle("~/Content/addcard").Include(
                "~/Content/AddCard.css"
            ));
            bundles.Add(new StyleBundle("~/Content/mybusidex").Include(
                "~/Content/MyBusidex.css"
            ));
            bundles.Add(new StyleBundle("~/Content/mygroups").Include(
                "~/Content/MyBusidex.css",
                "~/Content/groups.css"
            ));
            #endregion
        }
    }
}

//public class BusidexBundleOrderer : IBundleOrderer
//{
//    public virtual IEnumerable<FileInfo> OrderFiles(BundleContext context, IEnumerable<FileInfo> files)
//    {
//        //any ordering logic here
//        files = files.OrderBy(f => f.Name);
//        return files;
//    }
//}