using EmailService;
using Microsoft.AspNetCore.Mvc;
using OnlineBalance.Models.ViewModels;
using System.Diagnostics;

namespace OnlineBalance.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/send-email")]
        [HttpGet]
        public IActionResult SendEmail(string email)
        {
            Console.WriteLine(email + " retrieved");

            var emailMsg = new Message(new string[] { email }, "Subject of test email №1", "Test text №1");
            _emailSender.SendEmail(emailMsg);

            return Ok("Sent to " + email);
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