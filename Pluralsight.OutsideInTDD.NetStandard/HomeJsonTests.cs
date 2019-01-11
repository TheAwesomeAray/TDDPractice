using Microsoft.Owin.Hosting;
using Owin;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace Pluralsight.OutsideInTDD.NetStandard
{
    public class HomeJsonTests
    {
        [Fact]
        public void GetResponseReturnsCorrectStatusCode()
        {
            var baseAddress = "http://localhost:9000";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(baseAddress).Result;

                    Assert.True(response.IsSuccessStatusCode, "Actual Status Code: " + response.StatusCode);
                }
            }
        }

        public class Startup
        {
            public void Configure(IAppBuilder appBuilder)
            {
                var config = new HttpConfiguration();
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { controller = "Journal", id = RouteParameter.Optional }
                );

                appBuilder.UseWebApi(config);
            }
        }

        public class JournalController : ApiController
        {
        }

    }
}
