using System.Net.Http.Json;
using System;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using LinebotPoc.Shared;

namespace LinebotPoc.Client.Providers;
//[AddAuthorization]
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
    private HttpClient HttpClient;
    public CustomAuthStateProvider(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var response = await HttpClient.GetAsync("api/user/userInfo");
        if (response.IsSuccessStatusCode)
        {
            //ReadAsStringAsync 可以先讀出Json 再轉，比較好debug
            //var result = await response.Content.ReadFromJsonAsync<T>();
            string json = await response.Content.ReadAsStringAsync();
            if (json != "")
            {
                var userDto = JsonSerializer.Deserialize<DummyUserDto>(json);
                SetAuthInfo(userDto);
            }

        }
        return await GetClaimsPrincipalAsync();
    }

    public async Task<AuthenticationState> GetClaimsPrincipalAsync()
    {
        await Task.Delay(0);
        return new AuthenticationState(claimsPrincipal);
    }


    public void SetAuthInfo(DummyUserDto user)
    {
        var claims = new List<Claim>
        {
           new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.UserEmail),
        };
        var identity = new ClaimsIdentity(claims, "LinebotPocAuthCookie");
        claimsPrincipal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetClaimsPrincipalAsync());
    }

    public void ClearAuthInfo()
    {
        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetClaimsPrincipalAsync());
    }
}