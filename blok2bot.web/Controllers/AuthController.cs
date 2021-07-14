using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace blok2bot.web
{
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties 
            {
                RedirectUri = "/"
            }, "discord");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return LocalRedirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> GetGuilds()
        {
            HttpClient client = new HttpClient();

            string token = await HttpContext.GetTokenAsync("access_token");

            var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me/guilds");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            Response.StatusCode = 200;
            Response.ContentType = "application/json";


            return Ok(await response.Content.ReadAsStreamAsync());
            //return 

        }

    }
}
