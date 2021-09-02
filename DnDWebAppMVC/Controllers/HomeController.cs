using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DnDWebAppMVC.Data;
using DnDWebAppMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DnDWebAppMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AzureSQLDbContext _azureSQL;
        private readonly CosmosDbHelper _cosmosDbHelper;

        public HomeController(ILogger<HomeController> logger, AzureSQLDbContext azureSQL, CosmosDbHelper cosmosDbHelper)
        {
            _logger = logger;
            _azureSQL = azureSQL;
            _cosmosDbHelper = cosmosDbHelper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["Known"] = true;

                var profile = (await GetProfiles()).FirstOrDefault();
                if (profile == null)
                {
                    ViewData["Characters"] = null;
                }
                else
                {
                    ViewData["Characters"] = new SelectList(_cosmosDbHelper.Characters(profile.Id).Result, "Id", "Name");
                }
            }
            else
            {
                ViewData["Known"] = false;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<IEnumerable<UserProfile>> GetProfiles()
        {
            var userId = AuthHelper.GetOid(User);
            return await _azureSQL.UserProfiles
                .Where(p => p.AccountId == userId)
                .ToListAsync();
        }
    }
}
