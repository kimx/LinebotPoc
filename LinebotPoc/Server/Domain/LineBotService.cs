using LineBotLibrary.Dtos;
using LineBotLibrary.Enum;
using LineBotMessage.Dtos;
using LinebotPoc.Server.Common;
namespace LinebotPoc.Server.Domain;
public class LineBotService
{
    private readonly string SiteUrl = "";
    private UserService UserService;
    private LineBotApiClient LineBotApiClient;
    public LineBotService(UserService userService, LineBotApiClient lineBotApiClient,string siteUrl)
    {
        SiteUrl= siteUrl;
        UserService = userService;
        LineBotApiClient = lineBotApiClient;
    }

    #region  接收 webhook event 處理
    /// <summary>
    /// 接收 webhook event 處理
    /// </summary>
    /// <param name="requestBody"></param>
    public async Task ReceiveWebhook(WebhookRequestBodyDto requestBody)
    {
        dynamic replyMessage;
        foreach (var eventObject in requestBody.Events)
        {
            switch (eventObject.Type)
            {
                case WebhookEventTypeEnum.Message:
                    if (eventObject.Message.Type == MessageTypeEnum.Text)
                        ReceiveMessageWebhookEvent(eventObject);
                    //TODO: Other : 圖片、連結、聲音
                    break;

                case WebhookEventTypeEnum.Postback:
                    if (eventObject.Postback.Data == "BindERP")
                    {
                        await ReplyBindERP(eventObject);
                    }
                    break;
                case WebhookEventTypeEnum.AccountLink:
                    if (eventObject.Link.Result == "ok")
                    {
                        await ReplyBindERPComplete(eventObject);
                    }
                    break;
                case WebhookEventTypeEnum.Follow:
                    ReceiveMessageWebhookEvent(eventObject);
                    break;

            }
        }
    }

    private void ReceiveMessageWebhookEvent(WebhookEventDto eventDto)
    {
        dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();
        if (!IsBindUser(eventDto))
        {
            replyMessage = GetBindUserReply(eventDto);
        }
        else
        { // 關鍵字 : "help"
            if (eventDto.Message.Text == "/help")
            {
                replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                {
                    ReplyToken = eventDto.ReplyToken,
                    Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "您已挷定成功",
                                    QuickReply = new QuickReplyItemDto
                                    {
                                        Items = new List<QuickReplyButtonDto>
                                        {
                                            // uri action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "取得程式碼" ,
                                                    Uri = "https://github.com/kimx/LinebotPoc"
                                                }
                                            },
                                       
                                            // camera action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Camera,
                                                    Label = "開啟相機(測試)"
                                                }
                                            },
                                            // camera roll action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.CameraRoll,
                                                    Label = "開啟相簿(測試)"
                                                }
                                            },
                                            // location action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Location,
                                                    Label = "開啟位置(測試)"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                };
            }
            else
            {
                replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                {
                    ReplyToken = eventDto.ReplyToken,
                    Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "您已挷定成功，使用其它功能，請輸入: /help",
                                }
                            }
                };
            }

        }

        LineBotApiClient.ReplyMessage(replyMessage);
    }

    #endregion

    #region AccountLink Process
    private bool IsBindUser(WebhookEventDto eventDto)
    {
        string lineUserId = eventDto.Source.UserId;
        var user = UserService.GetUserByLine(lineUserId);
        return user != null;
    }

    private async Task ReplyBindERP(WebhookEventDto eventDto)
    {
        var linkToken = await LineBotApiClient.IssueLinkToken(eventDto);
        var replyMessage = new ReplyMessageRequestDto<TextMessageDto>
        {
            ReplyToken = eventDto.ReplyToken,
            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = $"{SiteUrl}login/{linkToken.LinkToken}"
                                }
                            }
        };

        await LineBotApiClient.ReplyMessage(replyMessage);
    }

    private async Task ReplyBindERPComplete(WebhookEventDto eventDto)
    {
        var find = UserService.GetUserByLineNonce(eventDto.Link.Nonce);
        find.LineUserId = eventDto.Source.UserId;
        UserService.Update(find);
        var replyMessage = new ReplyMessageRequestDto<TextMessageDto>
        {
            ReplyToken = eventDto.ReplyToken,
            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "帳號綁定完成!!",
                                }
                            }
        };
        await LineBotApiClient.ReplyMessage(replyMessage);
    }


    private ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>> GetBindUserReply(WebhookEventDto eventDto)
    {
        var replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>
        {
            ReplyToken = eventDto.ReplyToken,
            Messages = new List<TemplateMessageDto<ButtonsTemplateDto>>
                            {
                                new TemplateMessageDto<ButtonsTemplateDto>
                                {
                                    AltText = "挷定ERP帳號",
                                    Template = new ButtonsTemplateDto
                                    {
                                        //ThumbnailImageUrl = "https://i.imgur.com/8Bm1HLk.png",
                                        //ImageSize = TemplateImageSizeEnum.Contain,
                                        Title = "親愛的用戶您好，歡迎您使用本ERP系統",
                                        Text = "請先挷定ERP帳號。",
                                        Actions = new List<ActionDto>
                                        {
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "BindERP",
                                                Label = "要求挷定",
                                                DisplayText = "要求挷定"
                                            },

                                        }
                                    }
                                }
                            }
        };
        return replyMessage;
    }
    #endregion

    #region ReplyMessageRequestDto 範例參考
    //private void ReceiveMessageWebhookEvent(WebhookEventDto eventDto)
    //{
    //    dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();

    //    // 關鍵字 : "測試"
    //    if (eventDto.Message.Text == "測試")
    //    {
    //        // 回覆文字訊息並挾帶 quick reply button
    //        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
    //        {
    //            ReplyToken = eventDto.ReplyToken,
    //            Messages = new List<TextMessageDto>
    //                        {
    //                            new TextMessageDto
    //                            {
    //                                Text = "QuickReply 測試訊息",
    //                                QuickReply = new QuickReplyItemDto
    //                                {
    //                                    Items = new List<QuickReplyButtonDto>
    //                                    {
    //                                        // message action
    //                                        new QuickReplyButtonDto {
    //                                            Action = new ActionDto {
    //                                                Type = ActionTypeEnum.Message,
    //                                                Label = "message 測試" ,
    //                                                Text = "測試"
    //                                            }
    //                                        },
    //                                        // uri action
    //                                        new QuickReplyButtonDto {
    //                                            Action = new ActionDto {
    //                                                Type = ActionTypeEnum.Uri,
    //                                                Label = "uri 測試" ,
    //                                                Uri = "https://www.appx.com.tw"
    //                                            }
    //                                        },
    //                                         // 使用 uri schema 分享 Line Bot 資訊
    //                                        new QuickReplyButtonDto {
    //                                            Action = new ActionDto {
    //                                                Type = ActionTypeEnum.Uri,
    //                                                Label = "分享 Line Bot 資訊" ,
    //                                                Uri = "https://line.me/R/nv/recommendOA/@089yvykp"
    //                                            }
    //                                        },
    //                                        // postback action
    //                                        new QuickReplyButtonDto {
    //                                            Action = new ActionDto {
    //                                                Type = ActionTypeEnum.Postback,
    //                                                Label = "postback 測試" ,
    //                                                Data = "quick reply postback action" ,
    //                                                DisplayText = "使用者傳送 displayTex，但不會有 Webhook event 產生。",
    //                                                InputOption = PostbackInputOptionEnum.OpenKeyboard,
    //                                                FillInText = "第一行\n第二行"
    //                                            }
    //                                        },
    //                                        // datetime picker action
    //                                        new QuickReplyButtonDto {
    //                                            Action = new ActionDto {
    //                                            Type = ActionTypeEnum.DatetimePicker,
    //                                            Label = "日期時間選擇",
    //                                                Data = "quick reply datetime picker action",
    //                                                Mode = DatetimePickerModeEnum.Datetime,
    //                                                Initial = "2022-09-30T19:00",
    //                                                Max = "2022-12-31T23:59",
    //                                                Min = "2021-01-01T00:00"
    //                                            }
    //                                        },
    //                                        // camera action
    //                                        new QuickReplyButtonDto {
    //                                            Action = new ActionDto {
    //                                                Type = ActionTypeEnum.Camera,
    //                                                Label = "開啟相機"
    //                                            }
    //                                        },
    //                                        // camera roll action
    //                                        new QuickReplyButtonDto {
    //                                            Action = new ActionDto {
    //                                                Type = ActionTypeEnum.CameraRoll,
    //                                                Label = "開啟相簿"
    //                                            }
    //                                        },
    //                                        // location action
    //                                        new QuickReplyButtonDto {
    //                                            Action = new ActionDto {
    //                                                Type = ActionTypeEnum.Location,
    //                                                Label = "開啟位置"
    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                        }
    //        };
    //    }
    //    // 關鍵字 : "Sender"
    //    if (eventDto.Message.Text == "Sender")
    //    {
    //        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
    //        {
    //            ReplyToken = eventDto.ReplyToken,
    //            Messages = new List<TextMessageDto>
    //                        {
    //                            new TextMessageDto
    //                            {
    //                                Text = "你好，我是客服人員 1號",
    //                                Sender = new SenderDto
    //                                {
    //                                    Name = "客服人員 1號",
    //                                    IconUrl = "https://3a8e-114-37-157-213.jp.ngrok.io/UploadFiles/man.png"
    //                                }
    //                            },
    //                            new TextMessageDto
    //                            {
    //                                Text = "你好，我是客服人員 2號",
    //                                Sender = new SenderDto
    //                                {
    //                                    Name = "客服人員 2號",
    //                                    IconUrl = "https://3a8e-114-37-157-213.jp.ngrok.io/UploadFiles/gamer.png"
    //                                }
    //                            }
    //                        }
    //        };
    //    }
    //    // 關鍵字 : "Buttons"
    //    //if (eventDto.Message.Text == "Buttons")
    //    //{
    //    //    replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>
    //    //    {
    //    //        ReplyToken = eventDto.ReplyToken,
    //    //        Messages = new List<TemplateMessageDto<ButtonsTemplateDto>>
    //    //            {
    //    //                new TemplateMessageDto<ButtonsTemplateDto>
    //    //                {
    //    //                    AltText = "這是按鈕模板訊息",
    //    //                    Template = new ButtonsTemplateDto
    //    //                    {
    //    //                        ThumbnailImageUrl = "https://i.imgur.com/CP6ywwc.jpg",
    //    //                        ImageAspectRatio = TemplateImageAspectRatioEnum.Rectangle,
    //    //                        ImageSize = TemplateImageSizeEnum.Cover,
    //    //                        Title = "親愛的用戶您好，歡迎您使用本美食推薦系統",
    //    //                        Text = "請選擇今天想吃的主食，系統會推薦附近的餐廳給您。",
    //    //                        Actions = new List<ActionDto>
    //    //                        {
    //    //                            new ActionDto
    //    //                            {
    //    //                                Type = ActionTypeEnum.Postback,
    //    //                                Data = "foodType=sushi",
    //    //                                Label = "壽司",
    //    //                                DisplayText = "壽司"
    //    //                            },
    //    //                            new ActionDto
    //    //                            {
    //    //                                Type = ActionTypeEnum.Postback,
    //    //                                Data = "foodType=hot-pot",
    //    //                                Label = "火鍋",
    //    //                                DisplayText = "火鍋"
    //    //                            },
    //    //                            new ActionDto
    //    //                            {
    //    //                                Type = ActionTypeEnum.Postback,
    //    //                                Data = "foodType=steak",
    //    //                                Label = "牛排",
    //    //                                DisplayText = "牛排"
    //    //                            },
    //    //                            new ActionDto
    //    //                            {
    //    //                                Type = ActionTypeEnum.Postback,
    //    //                                Data = "foodType=next",
    //    //                                Label = "下一個",
    //    //                                DisplayText = "下一個"
    //    //                            }
    //    //                        }
    //    //                    }
    //    //                }
    //    //            }
    //    //    };
    //    //}
    //    //// 關鍵字 : "Confirm"
    //    //if (eventDto.Message.Text == "Confirm")
    //    //{
    //    //    replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ConfirmTemplateDto>>
    //    //    {
    //    //        ReplyToken = eventDto.ReplyToken,
    //    //        Messages = new List<TemplateMessageDto<ConfirmTemplateDto>>
    //    //            {
    //    //                new TemplateMessageDto<ConfirmTemplateDto>
    //    //                {
    //    //                    AltText = "這是確認模組訊息",
    //    //                    Template = new ConfirmTemplateDto
    //    //                    {
    //    //                        Text = "請問您是否喜歡本產品?\n(產品編號123)",
    //    //                        Actions = new List<ActionDto>
    //    //                        {
    //    //                            new ActionDto
    //    //                            {
    //    //                                Type = ActionTypeEnum.Postback,
    //    //                                Data = "id=123&like=yes",
    //    //                                Label = "喜歡",
    //    //                                DisplayText = "喜歡",
    //    //                            },
    //    //                            new ActionDto
    //    //                            {
    //    //                                Type = ActionTypeEnum.Postback,
    //    //                                Data = "id=123&like=no",
    //    //                                Label = "不喜歡",
    //    //                                DisplayText = "不喜歡",
    //    //                            }
    //    //                        }

    //    //                    }
    //    //                }

    //    //            }
    //    //    };
    //    //}
    //    //// 關鍵字 : "Carousel"
    //    //if (eventDto.Message.Text == "Carousel")
    //    //{
    //    //    replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<CarouselTemplateDto>>
    //    //    {
    //    //        ReplyToken = eventDto.ReplyToken,
    //    //        Messages = new List<TemplateMessageDto<CarouselTemplateDto>>
    //    //            {
    //    //                new TemplateMessageDto<CarouselTemplateDto>
    //    //                {
    //    //                    AltText = "這是輪播訊息",
    //    //                    Template = new CarouselTemplateDto
    //    //                    {
    //    //                        Columns = new List<CarouselColumnObjectDto>
    //    //                        {
    //    //                            //column objects
    //    //                            new CarouselColumnObjectDto
    //    //                            {
    //    //                                ThumbnailImageUrl = "https://www.apple.com/v/iphone-14-pro/a/images/meta/iphone-14-pro_overview__e2a7u9jy63ma_og.png",
    //    //                                Title = "全新上市 iPhone 14 Pro",
    //    //                                Text = "現在購買享優惠，全品項 9 折",
    //    //                                Actions = new List<ActionDto>
    //    //                                {
    //    //                                    // 按鈕 action
    //    //                                    new ActionDto
    //    //                                    {
    //    //                                        Type = ActionTypeEnum.Uri,
    //    //                                        Label ="立即購買",
    //    //                                        Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
    //    //                                    }
    //    //                                }
    //    //                            },
    //    //                        }
    //    //                    }
    //    //                }
    //    //            }
    //    //    };
    //    //}
    //    //// 關鍵字 : "ImageCarousel"
    //    //if (eventDto.Message.Text == "ImageCarousel")
    //    //{
    //    //    replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ImageCarouselTemplateDto>>
    //    //    {
    //    //        ReplyToken = eventDto.ReplyToken,
    //    //        Messages = new List<TemplateMessageDto<ImageCarouselTemplateDto>>
    //    //            {
    //    //                new TemplateMessageDto<ImageCarouselTemplateDto>
    //    //                {
    //    //                    AltText = "這是圖片輪播訊息",
    //    //                    Template = new ImageCarouselTemplateDto
    //    //                    {
    //    //                        Columns = new List<ImageCarouselColumnObjectDto>
    //    //                        {
    //    //                            new ImageCarouselColumnObjectDto
    //    //                            {
    //    //                                ImageUrl = "https://i.imgur.com/74I9rlb.png",
    //    //                                Action = new ActionDto
    //    //                                {
    //    //                                    Type = ActionTypeEnum.Uri,
    //    //                                    Label = "前往官網",
    //    //                                    Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
    //    //                                }
    //    //                            },
    //    //                            new ImageCarouselColumnObjectDto
    //    //                            {
    //    //                                ImageUrl = "https://www.apple.com/v/iphone-14-pro/a/images/meta/iphone-14-pro_overview__e2a7u9jy63ma_og.png",
    //    //                                Action = new ActionDto
    //    //                                {
    //    //                                    Type = ActionTypeEnum.Uri,
    //    //                                    Label = "立即購買",
    //    //                                    Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
    //    //                                }
    //    //                            },

    //    //                        }
    //    //                    }
    //    //                }
    //    //            }
    //    //    };
    //    //}
    //    ReplyMessageHandler(replyMessage);
    //}
    #endregion



}