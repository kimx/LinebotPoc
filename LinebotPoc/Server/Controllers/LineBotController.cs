using LineBotLibrary.Dtos;
using LinebotPoc.Server.Common;
using LinebotPoc.Server.Domain;
using Microsoft.AspNetCore.Mvc;

namespace LinebotPoc.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineBotController : ControllerBase
    {
        private readonly LineBotService _lineBotService;
        private readonly LineBotApiClient _lineBotApiClient;
        // constructor
        public LineBotController(IUserService userService, LineBotApiClient lineBotApiClient, IHttpContextAccessor httpContextAccessor)
        {
            string siteUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host.Value}/";
            _lineBotService = new LineBotService(userService, lineBotApiClient, siteUrl);
            _lineBotApiClient = lineBotApiClient;
        }
      
        //https://linebotpoc.azurewebsites.net/api/LineBot/Webhook
        [HttpPost("Webhook")]
        public Task Webhook(WebhookRequestBodyDto body)
        {
            return _lineBotService.ReceiveWebhook(body);

        }

        //For Balzor Client呼叫，因Blazor WAM CORS Origin無法直接呼叫Line API
        [HttpPost("PushMessage")]
        public Task<string> PushMessage(PushMessageRequestDto<TextMessageDto> request)
        {
            var result = _lineBotApiClient.PushMessage(request);
            return result;
        }

        [HttpPost("MulticastMessage")]
        public Task<string> MulticastMessage(MulticastMessageRequestDto<TextMessageDto> request)
        {
            var result = _lineBotApiClient.MulticastMessage(request);
            return result;
        }

        [HttpPost("BroadcastMessage")]
        public Task<string> BroadcastMessage(BroadcastMessageRequestDto<TextMessageDto> request)
        {
            var result = _lineBotApiClient.BroadcastMessage(request);
            return result;
        }
    }
}