using LinebotPoc.Server.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc;
using LinebotPoc.Server.Common;
using NLog.Web;
using LinebotPoc.Server.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("Kim__LinePoc__");//環境變數
// Add services to the container.
builder.Logging.ClearProviders();
builder.WebHost.UseNLog();

builder.Services.AddRazorPages();
builder.Services.AddScoped(sp =>
{
    string channelAccessToken = builder.Configuration["ChannelAccessToken"];
    LineBotApiClient lineBotApiClient = new LineBotApiClient(channelAccessToken, "");
    return lineBotApiClient;
});
builder.Services.AddScoped<UserService>();
//[auth]
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme
).AddCookie();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, LinebotPoc.Server.Common.AuthorizationMiddlewareResultHandler>();
builder.Services.AddControllersWithViews(options =>
{
    // options.ModelBinderProviders.Insert(0, new BaseDtoBinderProvider());
    //將非null自動視為Required的設定停用,例:string Memo:預設非null，會加上[Required]
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    options.Filters.Add<ApiExceptionFilter>();
    options.Filters.Add<LogFilter>();

}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;//保持原本的屬性名稱,大小寫不更改
}); ;

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
//[auth]
app.UseAuthentication();
app.UseAuthorization();
app.Run();
