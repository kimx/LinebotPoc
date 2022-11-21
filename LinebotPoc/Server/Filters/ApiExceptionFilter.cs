using LinebotPoc.Server.Pages;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LinebotPoc.Server.Filters;
public class ApiExceptionFilter : IExceptionFilter
{
    private ILogger<ApiExceptionFilter> _log;
    public ApiExceptionFilter(ILogger<ApiExceptionFilter> log)
    {
        _log = log;

    }

    public void OnException(ExceptionContext context)
    {
        _log.LogError(context.Exception, context.Exception.Message);


    }




}
