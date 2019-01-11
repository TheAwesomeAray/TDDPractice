using System;
using System.Net.Http;
using Xunit;

namespace Pluralsight.OutsideInTDD
{
    public class HomeJsonTests
    {
        [Fact]
        public void GetResponseReturnsCorrectStatusCode()
        {
            var baseAddress = "https://localhost:44340/api/journal";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(baseAddress).Result;

                Assert.True(response.IsSuccessStatusCode, "Actual Status Code: " + response.StatusCode);
            }
        }
    }

    //public class Startup
    //{
    //    public void Configure(IAppBuilder appBuilder)
    //    {
    //        var config = new HttpConfiguration();
    //        config.Routes.MapHttpRoute(
    //            name: "DefaultApi",
    //            routeTemplate: "api/{controller}/{id}",
    //            defaults: new { controller = "Journal", id = RouteParameter.Optional }
    //        );

    //        appBuilder.UseWebApi(config);
    //    }
    //}

    //public class JournalController : ApiController
    //{
    //}

}
