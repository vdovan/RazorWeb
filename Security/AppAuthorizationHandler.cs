using Microsoft.AspNetCore.Authorization;

namespace AppMvc.Security
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        ILogger<AppAuthorizationHandler> _logger;
        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger)
        {
            _logger = logger;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if(context.Resource != null)
            {
                DefaultHttpContext httpContext = (DefaultHttpContext)context.Resource;
                _logger.LogInformation("Request path ~ " + httpContext.Request.Path);
                _logger.LogInformation("Request path ~ " + Newtonsoft.Json.JsonConvert.SerializeObject(""));
                // if(httpContext.Request.Method == "POST")
                // {
                //     IFormCollection _form = httpContext.Request.Form;
                //     if(_form != null)
                //         _logger.LogInformation("Form request " + Newtonsoft.Json.JsonConvert.SerializeObject(_form));
                // }
            }
            foreach(var requiement in context.Requirements)
            {
                if(requiement is AppRequirement)
                {
                    context.Succeed(requiement);
                }
            }
            return Task.CompletedTask;
        }
    }
}