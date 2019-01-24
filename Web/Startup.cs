/*
    Author      : Eric Richard Widmann
    Date        : 1/23/2019
    Description :
        Set's up rest api and http routing.
*/
using System;
using Owin;
using System.Web.Http;
using System.Net.Http;

namespace Ledger.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration CONFIG = new HttpConfiguration();
            CONFIG.EnableCors();
            CONFIG.Routes.MapHttpRoute(
                name: "createUserApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            appBuilder.UseWebApi(CONFIG);
        }
    }
}
