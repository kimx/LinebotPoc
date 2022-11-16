using LinebotPoc.Server.Pages;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LinebotPoc.Server.Filters;
public class LogFilter : IActionFilter
{
    private ILogger<LogFilter> _log;
    public LogFilter(ILogger<LogFilter> log)
    {
        _log = log;
    }
    public void OnActionExecuting(ActionExecutingContext context)
    {
        _log.LogInformation($"{context.ActionDescriptor.DisplayName}");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }





}
