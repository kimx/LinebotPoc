using LinebotPoc.Server.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc;
using LinebotPoc.Server.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews()
    //API Controller��Json�]�w
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;//�O���쥻���ݩʦW��,�j�p�g�����
    });
builder.Services.AddRazorPages();
builder.Services.AddScoped<LineBotHelper>();
builder.Services.AddScoped<UserService>();
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

});



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
