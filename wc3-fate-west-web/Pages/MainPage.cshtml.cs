using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using wc3_fate_west_data_access_layer.Data;
using Microsoft.AspNetCore.Hosting;
using wc3_fate_west_web.Models;

namespace wc3_fate_west_web.Pages
{
    public class MainPageModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly wc3_fate_west_data_access_layer.FateWestDbContext _context;
        [BindProperty]
        public List<string> ImageList { get; set; }
        public string CurrentBotTime { get => DateTime.Now.ToString(); }
        public MainPageModel(wc3_fate_west_data_access_layer.FateWestDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public IEnumerable<GameData> RecentGameDataList { get => new GameSL(_context).GetRecentGames(10); }
        public IActionResult GetImages()
        {
            // get the real path of wwwroot/imagesFolder
            var rootDir = this._webHostEnvironment.WebRootPath;
            // the extensions allowed to show
            var filters = new String[] { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".bmp", ".svg" };
            // set the base url = "/"
            var baseUrl = "/";


            var imgUrls = Directory.EnumerateFiles(rootDir, "*.*", SearchOption.AllDirectories)
                .Where(fileName => filters.Any(filter => fileName.EndsWith(filter)))
                .Select(fileName => Path.GetRelativePath(rootDir, fileName)) // get relative path
                .Select(fileName => Path.Combine(baseUrl, fileName))          // prepend the baseUrl
                .Select(fileName => fileName.Replace("\\", "/"))                // replace "\" with "/"
                ;
            return new JsonResult(imgUrls);
        }
        public void OnGet()
        {
        }
    }
}