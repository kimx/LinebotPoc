# Line Bot 針對需求的功能測試
* 挷定企業使用者
* 送訊息給全部的使用者
* 發送訊息給特定使用者

# 線上測試
* [測試站台](https://linebotpoc.azurewebsites.net/)
* 掃碼加入  

![image](https://user-images.githubusercontent.com/5724118/201879056-c55a7f46-d8d3-4c28-98d0-8af8d29ca745.png)


# Bot挷定畫面
![image](https://user-images.githubusercontent.com/5724118/201878205-7b84b818-1e1f-45c7-bbf5-f9d6ea184715.png)
![image](https://user-images.githubusercontent.com/5724118/201878328-65128a6c-443b-4219-b12a-15edc2c64d90.png)
![image](https://user-images.githubusercontent.com/5724118/201878389-dd2eb21c-6a25-47e2-ac93-6212dcf9dd49.png)


# 後台通知功能
![image](https://user-images.githubusercontent.com/5724118/201876996-435bdef0-bb5d-4070-b45f-1817b34383f1.png)

# 測試程式碼
* 下載程式碼，你須修改如下:
* Program.cs 修改 channelAccessToken

  builder.Services.AddScoped(sp =>  
  {  
    string channelAccessToken = "你的Token";  
    LineBotApiClient lineBotApiClient = new LineBotApiClient(channelAccessToken, "");  
    return lineBotApiClient;  
  });

*LineBotService.cs 修改SiteUrl

    private readonly string SiteUrl = "https://your site/";



# 參考來源
* [讓 C# 也可以很 Social - 在 .NET 6 用 C# 串接 LINE Services API 的取經之路 系列](https://ithelp.ithome.com.tw/users/20151616/ironman/5866?page=1)
  > 大部份的程式參考都來自此篇作者提供的範例程式
* [如何在 LINE Bot 整合網站帳號 - Account Link](https://ithelp.ithome.com.tw/articles/10229907)
* [LineBot Message API 文件](https://developers.line.biz/en/reference/messaging-api/)

# 備註
* 主要為測試功能為主，程式碼東拼西湊而來，請小心服用....
