using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RazorWeb.Models;
using CommonHelper;
using RazorWeb.Constants;

namespace RazorWeb.MiddleWare
{
    public class LoggingMiddleWare : ActionFilterAttribute
    {
        private Dictionary<string, string> strConnList = new Dictionary<string, string>();

        public override void OnResultExecuted(Microsoft.AspNetCore.Mvc.Filters.ResultExecutedContext resContext)
        {
            var rq = resContext.HttpContext.Request;
            var rp = resContext.HttpContext.Response;
            string test = string.Empty;

            if (rq.Method == "POST")
            {
                IFormCollection _form = rq.Form;

                if (_form != null)
                {
                    var formbody = _form.Where(x => x.Key != "__RequestVerificationToken");
                    test = Newtonsoft.Json.JsonConvert.SerializeObject(formbody);
                    Console.WriteLine("OnResultExecuted formdata: " + test);
                }

            }
            if (resContext.HttpContext.Response.StatusCode == 302)
            {
                Console.WriteLine("OnResultExecuted 302: ");
                var _db = resContext.HttpContext.RequestServices.GetService<AppDbContext>();

                _db.articles.Add(new Article
                {
                    Title = "LOG",
                    Content = $"Method {rq.Method} path {rq.Path} {test}"
                });
                _db.SaveChanges();
            }
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            // Console.WriteLine("OnActionExecuting: ");
            // HttpContext con = context.HttpContext;
            // HttpRequest rq = context.HttpContext.Request;
            // Console.ForegroundColor = ConsoleColor.Green;
            // Console.WriteLine("OnActionExecuting: " + con.TraceIdentifier);
            // Console.ResetColor();
        }
    }
}