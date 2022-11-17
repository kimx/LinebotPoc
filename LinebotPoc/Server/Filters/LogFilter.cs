using LinebotPoc.Server.Pages;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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
        try
        {
            string json = JsonSerializer.Serialize(context.ActionArguments);
            _log.LogInformation($"{context.ActionDescriptor.DisplayName},{json}");
        }
        catch (Exception ex)
        {
            _log.LogDebug($"{context.ActionDescriptor.DisplayName},{ex}");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }





}
