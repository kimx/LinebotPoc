using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace LinebotPoc.Server.Common
{
    /// <summary>
    /// For API Cookie驗證失敗時的自訂處理，預設Cookie的驗證會道到Account/Login，這裡處理為回傳401
    ///[AddAuthorization]
    /// </summary>
    public class AuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            //驗證失敗回傳401
            if (!authorizeResult.Succeeded)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
            //授權成功要繼續執行
            await next(context);
        }
    }
}
