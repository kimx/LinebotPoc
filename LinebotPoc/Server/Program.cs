using LinebotPoc.Server.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc;
using LinebotPoc.Server.Common;
using NLog.Web;
using LinebotPoc.Server.Filters;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("Kim__LinePoc__");//���������ܼƫe�m��
// Add services to the container.
builder.Logging.ClearProviders();
builder.WebHost.UseNLog();

builder.Services.AddRazorPages();
builder.Services.AddScoped(sp =>
{
    LineBotApiClient lineBotApiClient = new LineBotApiClient(builder.Configuration["ChannelAccessToken"], "");
    return lineBotApiClient;
});
string azureCosmosConnectionString = builder.Configuration["AzureCosmosConnectionString"];
if (!string.IsNullOrEmpty(azureCosmosConnectionString))
{
    builder.Services.AddSingleton<IUserService, CosmosUserService>();
}
else
    builder.Services.AddSingleton<IUserService, FileUserService>();


//[auth]
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme
).AddCookie();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, LinebotPoc.Server.Common.AuthorizationMiddlewareResultHandler>();
builder.Services.AddControllersWithViews(options =>
{
    // options.ModelBinderProviders.Insert(0, new BaseDtoBinderProvider());
    //�N�Dnull�۰ʵ���Required���]�w����,��:string Memo:�w�]�Dnull�A�|�[�W[Required]
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    options.Filters.Add<ApiExceptionFilter>();
    options.Filters.Add<LogFilter>();

}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;//�O���쥻���ݩʦW��,�j�p�g�����
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

if (!string.IsNullOrEmpty(azureCosmosConnectionString))
{
    await app.Services.GetRequiredService<IUserService>().InitAsync(azureCosmosConnectionString);
}
app.Run();

