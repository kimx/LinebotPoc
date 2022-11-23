using LineBotLibrary.Dtos;
using LineBotLibrary.Dtos.Messages;
using LineBotLibrary.Dtos.Profile;
using LineBotLibrary.Enum;
using LineBotLibrary.Providers;
using LineBotMessage.Dtos;
using System;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace LinebotPoc.Server.Common;
//https://developers.line.biz/en/reference/messaging-api/
public class LineBotApiClient
{
    #region Field
    // 貼上 messaging api channel 中的 accessToken & secret
    private string ChannelAccessToken = "";
    private string ChannelSecret = "";//未用到暫留
    private static HttpClient client = new HttpClient();
    private readonly JsonProvider _jsonProvider = new JsonProvider();
    #endregion

    #region Line API Url
    private readonly string GetUserProfileUri = "https://api.line.me/v2/bot/profile/{0}";

    private readonly string ReplyMessageUri = "https://api.line.me/v2/bot/message/reply";
    //1對全部
    private readonly string BroadcastMessageUri = "https://api.line.me/v2/bot/message/broadcast";

    //1對多 最多150
    private readonly string MulticastMessageUri = "https://api.line.me/v2/bot/message/multicast";
    //1對1
    private readonly string PushMessageUri = "https://api.line.me/v2/bot/message/push";

    //要求帳號連結Token
    private readonly string IssueLinkTokenUri = "https://api.line.me/v2/bot/user/{0}/linkToken";
    //與Line 對話帳號連結URL
    private readonly string AccountLinkUri = "https://access.line.me/dialog/bot/accountLink?linkToken={0}&nonce={1}";
    #endregion

    public LineBotApiClient(string channelAccessToken, string channelSecret)
    {
        ChannelAccessToken = channelAccessToken;
        ChannelSecret = channelSecret;
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChannelAccessToken); //帶入 channel access token

    }

    #region Common
    private async Task<string> SendAsync(string url, object request)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
        };
        if (request != null)
        {
            var json = _jsonProvider.Serialize(request);
            requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        var response = await client.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }


    private async Task<string> SendAsync(HttpRequestMessage requestMessage)
    {
        var response = await client.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }


    #endregion

    /// <summary>
    /// 取得使用者基本資訊
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    public async Task<UserProfileDto> GetUserProfile(string userId)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(string.Format(GetUserProfileUri, userId))
        };
        var content = await SendAsync(requestMessage);
        var profile = _jsonProvider.Deserialize<UserProfileDto>(content);
        return profile;
    }

    #region Account Link
    public string GetAccountLinkUrl(string linkToken, string nonce)
    {
        string url = string.Format(AccountLinkUri, linkToken, nonce);
        return url;

    }

    public async Task<LinkTokenDto> IssueLinkToken(WebhookEventDto eventDto)
    {
        var content = await SendAsync(string.Format(IssueLinkTokenUri, eventDto.Source.UserId), null);
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
        var content = await SendAsync(PushMessageUri, request);
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
        var content = await SendAsync(MulticastMessageUri, request);
        return content;
    }
    #endregion

    #region ReplyMessage
    /// <summary>
    /// 將回覆訊息請求送到 Line
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    public async Task<string> ReplyMessage<T>(ReplyMessageRequestDto<T> request)
    {
        var content = await SendAsync(ReplyMessageUri, request);
        return content;
    }
    #endregion

    #region BroadcastMessages
    /// <summary>
    /// 接收到廣播請求時，在將請求傳至 Line 前多一層處理，依據收到的 messageType 將 messages 轉換成正確的型別，這樣 Json 轉換時才能正確轉換。
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="requestBody"></param>
    public Task<string> BroadcastMessageHandler(string messageType, object requestBody)
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
        return BroadcastMessage(messageRequest);

    }

    /// <summary>
    /// 將廣播訊息請求送到 Line
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    public async Task<string> BroadcastMessage<T>(BroadcastMessageRequestDto<T> request)
    {
        var content = await SendAsync(BroadcastMessageUri, request);
        return content;
    }
    #endregion
}

