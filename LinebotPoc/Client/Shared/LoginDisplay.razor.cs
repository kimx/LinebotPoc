using LinebotPoc.Client.Providers;
using LinebotPoc.Shared.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

namespace LinebotPoc.Client.Shared;
public partial class LoginDisplay
{

    [Inject]
    AuthenticationStateProvider _authStateProvider { get; set; }
    [Inject]
    NavigationManager _navigationManager { get; set; }

    [Inject]
    HttpClient httpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {

        await base.OnInitializedAsync();
    }


    private async Task SignOut()
    {

        var response = await httpClient.PostAsJsonAsync("api/user/logout", new { });
        (_authStateProvider as CustomAuthStateProvider).ClearAuthInfo();
        _navigationManager.NavigateTo("/login");
    }


}

