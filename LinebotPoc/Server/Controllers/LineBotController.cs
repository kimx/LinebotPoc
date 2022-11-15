using LinebotPoc.Server.Common;
using LinebotPoc.Server.Domain;
using LinebotPoc.Shared;
using LinebotPoc.Shared.Dtos;
using LinebotPoc.Shared.Providers;
using Microsoft.AspNetCore.Mvc;

namespace LinebotPoc.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineBotController : ControllerBase
    {
        private readonly LineBotService _lineBotService;
        private readonly JsonProvider _jsonProvider;
        // constructor
        public LineBotController(UserService userService, LineBotHelper lineBotHelper)
        {
            _lineBotService = new LineBotService(userService, lineBotHelper);
            _jsonProvider = new JsonProvider();
        }

        //https://linebotpoc.azurewebsites.net/api/LineBot/Webhook
        [HttpPost("Webhook")]
        public IActionResult Webhook(WebhookRequestBodyDto body)
        {
            _lineBotService.ReceiveWebhook(body);
            return Ok();
        }

        [HttpPost("PushMessage")]
        public Task<string> PushMessage(PushMessageRequestDto<TextMessageDto> request)
        {
            var result = _lineBotService.PushMessage(request);
            return result;
        }

        [HttpPost("MulticastMessage")]
        public Task<string> MulticastMessage(MulticastMessageRequestDto<TextMessageDto> request)
        {
            var result = _lineBotService.MulticastMessage(request);
            return result;
        }

        [HttpPost("BroadcastMessage")]
        public Task<string> BroadcastMessage(BroadcastMessageRequestDto<TextMessageDto> request)
        {
            var result = _lineBotService.BroadcastMessage(request);
            return result;
        }

        //[HttpGet("TestPush")]
        //public Task<string> TestPush()
        //{
        //    PushMessageRequestDto<TextMessageDto> request = new Shared.Dtos.PushMessageRequestDto<TextMessageDto>();
        //    request.UserId = "Ueb31ba9bf0a13121b4db929abb610ce2";
        //    request.Messages = new List<TextMessageDto>();
        //    request.Messages.Add(new TextMessageDto { Text = "Push" });
        //    var result = _lineBotService.PushMessage(request);
        //    return result;
        //}

        //[HttpGet("TestLinkToken")]
        //public async Task<string> TestLinkToken()
        //{
        //    WebhookEventDto eventDto = new WebhookEventDto();
        //    eventDto.Source = new SourceDto();
        //    eventDto.Source.UserId = "Ueb31ba9bf0a13121b4db929abb610ce2";
        //    var result = await _lineBotService.IssueLinkToken(eventDto);
        //    return result.LinkToken;
        //}
    }
}