﻿using LinebotPoc.Server.Domain;
using LinebotPoc.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace LinebotPoc.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserService UserService;
        IConfiguration Configuration;
        public UserController(IUserService userService, IConfiguration configuration)
        {
            this.UserService = userService;
            this.Configuration = configuration;
        }

        #region Login相關
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginDto login)
        {
            var user = UserService.GetUser(login.UserEmail);
            if (user == null)
            {
                return BadRequest("Invalid Credentials");
            }

            var claims = new List<Claim>
        {
           new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.UserEmail),
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            return Ok(user);
        }

        /// <summary>
        /// 從目前的登入Cookie取回使用者資訊，讓Blazor 第一次讀取時驗證(CustomAuthStateProvider.cs)
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("userInfo")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = new DummyUserDto();
            user.UserName = HttpContext.User.Identity.Name;
            user.UserEmail = HttpContext.User.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value;
            return Ok(user);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return Ok(true);
        }

        #endregion


        [HttpGet("GetUsers")]
        public List<DummyUserDto> GetUsers()
        {
            return UserService.GetUsers();
        }

        [HttpPost("Add")]
        public Task Add(DummyUserDto userDto)
        {
            var find = UserService.GetUser(userDto.UserEmail);
            if (find != null)
            {
                throw new Exception("使用者已存在");
            }
            return UserService.Create(userDto);
        }

        [HttpPost("Update")]
        public Task Update(DummyUserDto userDto)
        {
            return UserService.Update(userDto);
        }

        [HttpPost("Delete")]
        public Task Delete(DummyUserDto userDto)
        {
            return UserService.Delete(userDto);
        }

        //Test Config
        [HttpGet("TestConfig")]
        public string[] TestConfig()
        {
            return new string[] {
                  Configuration["ChannelAccessToken"]
                , Configuration["InstrumentationKey"]
                 ,Configuration["ApplicationInsights:ConnectionString"]};
        }
    }
}
