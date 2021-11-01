using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data;
using System.Data.SqlClient;

namespace ICE
{



    public class MvcApplication : System.Web.HttpApplication
    {

        public string strIceConnection;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            strIceConnection = ConfigurationManager.ConnectionStrings["IceConnectionString"].ConnectionString;

            GlobalFilters.Filters.Add(new IceActionFilterAttribute());

        }
    }

    internal class IceActionFilterAttribute : ActionFilterAttribute
    {

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);

            Loggit(0, context.Controller.ToString(), "requested");   // 0 = trace
        }

    }

    public void Loggit(int intLogLevel, string strController, string strMessage)
    {

        using (SqlConnection connection = new SqlConnection(strIceConnection))
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "log.Loggit";  // assuming logging SPs have their own schema called log
                command.Parameters.AddWithValue("@LogLevel", intLogLevel);
                command.Parameters.AddWithValue("@Controller", strController);
                command.Parameters.AddWithValue("@Message", strMessage);
                command.ExecuteNonQuery();  // inserts a row into log.log, the timestamp comes from a column default of GetDate()
            }
        }

    }

}
