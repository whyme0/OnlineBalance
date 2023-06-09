﻿using Microsoft.AspNetCore.Mvc;
using OnlineBalance.Models;
using OnlineBalance.Models.ViewModels;
using System.Diagnostics;

namespace OnlineBalance.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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

        [Route("/notfound")]
        public IActionResult ResourceNotFound()
        {
            return View();
        }
    }
}