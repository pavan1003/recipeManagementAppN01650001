using System.Web;
using System.Web.Mvc;

namespace recipeManagementAppN01650001
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
