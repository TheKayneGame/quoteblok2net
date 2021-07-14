
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using quoteblok2net.quotes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace blok2bot
{
    public class Guild
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public Guild(string Name, string Id)
        {
            this.Name = Name;
            this.Id = Id;
        }

    }
    public class DiscordManager
    {
        public DiscordManager(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IQuoteManager quoteManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.httpClient = httpClient;
            this.quoteManager = quoteManager;
        }

        public IHttpContextAccessor httpContextAccessor { get; set; }
        
        public HttpClient httpClient { get; set; }
        
        public IQuoteManager quoteManager { get; set; }

        private string GetToken()
        {
            return httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
        }

        public List<Guild> GetGuilds()
        {
            List<Guild> guilds = new List<Guild>();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me/guilds");

            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
            HttpResponseMessage response = httpClient.SendAsync(request).Result;
            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            JsonDocument document = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);
            var guildsJson = document.RootElement.EnumerateArray();
            foreach (var guild in guildsJson)
            {
                if (quoteManager.GuildExists(Convert.ToUInt64(guild.GetProperty("id").ToString())))
                    guilds.Add(new Guild(guild.GetProperty("name").ToString(), guild.GetProperty("id").ToString()));
            }

            return guilds;
        }
    }
}
