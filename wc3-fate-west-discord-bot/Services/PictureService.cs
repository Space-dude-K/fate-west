﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace wc3_fate_west_discord_bot.Services
{
    public class PictureService
    {
        private readonly HttpClient _http;

        public PictureService(HttpClient http)
            => _http = http;

        public async Task<Stream> GetCatPictureAsync()
        {
            var resp = await _http.GetAsync("https://cataas.com/cat");
            return await resp.Content.ReadAsStreamAsync();
        }
    }
}