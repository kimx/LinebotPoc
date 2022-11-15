using LineBotMessage.Dtos;
using LinebotPoc.Shared.Dtos;
using LinebotPoc.Shared.Dtos.Messages;
using LinebotPoc.Shared.Enum;
using LinebotPoc.Shared.Providers;
using System.Net.Http.Headers;
using System.Text;

namespace LinebotPoc.Server.Common;
public class LineBotHelper
{
    #region Properties
    // 貼上 messaging api channel 中的 accessToken & secret
    private readonly string channelAccessToken = "";

    private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";

    //1對全部
    private readonly string broadcastMessageUri = "https://api.line.me/v2/bot/message/broadcast";

    //1對多 最多150
    private readonly string multicastMessageUri = "https://api.line.me/v2/bot/message/multicast";

    //1對1
    private readonly string pushMessageUri = "https://api.line.me/v2/bot/message/push";
    private readonly string issueLinkTokenUri = "https://api.line.me/v2/bot/user/{0}/linkToken";


    private static HttpClient client = new HttpClient();
    private readonly JsonProvider _jsonProvider = new JsonProvider();
    #endregion

    #region Common
    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
        var response = await client.SendAsync(requestMessage);
        return response;
    }

    public async Task<LinkTokenDto> IssueLinkToken(WebhookEventDto eventDto)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(string.Format(issueLinkTokenUri, eventDto.Source.UserId)),
        };

        var response = await SendAsync(requestMessage);
        var content = await response.Content.ReadAsStringAsync();
        LinkTokenDto linkTokenDto = _jsonProvider.Deserialize<LinkTokenDto>(content);
        return linkTokenDto;
    }
    #endregion

    #region Push
    /// <summary>
    /// 將回覆訊息請求送到 Line
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    public async Task<string> PushMessage<T>(PushMessageRequestDto<T> request)
    {
        var json = _jsonProvider.Serialize(request);
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(pushMessageUri),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await SendAsync(requestMessage);
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
    #endregion

    #region Multicast
    /// <summary>
    /// 將回覆訊息請求送到 Line
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    public async Task<string> MulticastMessage<T>(MulticastMessageRequestDto<T> request)
    {
        var json = _jsonProvider.Serialize(request);
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(pushMessageUri),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var response = await SendAsync(requestMessage);
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
    #endregion

    #region ReplyMessage
    /// <summary>
    /// 將回覆訊息請求送到 Line
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    public async void ReplyMessage<T>(ReplyMessageRequestDto<T> request)
    {
        var json = _jsonProvider.Serialize(request);
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(replyMessageUri),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await SendAsync(requestMessage);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    #endregion

    #region BroadcastMessages
    /// <summary>
    /// 接收到廣播請求時，在將請求傳至 Line 前多一層處理，依據收到的 messageType 將 messages 轉換成正確的型別，這樣 Json 轉換時才能正確轉換。
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="requestBody"></param>
    public void BroadcastMessageHandler(string messageType, object requestBody)
    {
        string strBody = requestBody.ToString();
        dynamic messageRequest = new BroadcastMessageRequestDto<BaseMessageDto>();
        switch (messageType)
        {
            case MessageTypeEnum.Text:
                messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<TextMessageDto>>(strBody);
                break;

                //case MessageTypeEnum.Sticker:
                //    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<StickerMessageDto>>(strBody);
                //    break;

                //case MessageTypeEnum.Image:
                //    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<ImageMessageDto>>(strBody);
                //    break;

                //case MessageTypeEnum.Video:
                //    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<VideoMessageDto>>(strBody);
                //    break;

                //case MessageTypeEnum.Audio:
                //    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<AudioMessageDto>>(strBody);
                //    break;

                //case MessageTypeEnum.Location:
                //    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<LocationMessageDto>>(strBody);
                //    break;

                //case MessageTypeEnum.Imagemap:
                //    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<ImagemapMessageDto>>(strBody);
                //    break;

                //case MessageTypeEnum.FlexBubble:
                //    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<FlexMessageDto<FlexBubbleContainerDto>>>(strBody);
                //    break;

                //case MessageTypeEnum.FlexCarousel:
                //    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<FlexMessageDto<FlexCarouselContainerDto>>>(strBody);
                //    break;
        }
        BroadcastMessage(messageRequest);

    }

    /// <summary>
    /// 將廣播訊息請求送到 Line
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    public async Task<string> BroadcastMessage<T>(BroadcastMessageRequestDto<T> request)
    {
        var json = _jsonProvider.Serialize(request);
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(broadcastMessageUri),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await SendAsync(requestMessage);
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

    #endregion
}

