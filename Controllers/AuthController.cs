using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBalance.Data;
using OnlineBalance.Mapping;
using OnlineBalance.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace OnlineBalance.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger _logger;
        private readonly UserMapping _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;

        public AuthController(ApplicationDbContext dbContext, ILogger<AuthController> logger, UserMapping mapper, UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Signup(string? redirectUrl = null)
        {
            TempData["redirectUrl"] = redirectUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup([FromForm] CreateUserDTO userDTO, string? redirectUrl = "signin/")
        {
            if (!ModelState.IsValid)
            {
                return View(userDTO);
            }

            User user = _mapper.MapUserRegistration(new User(), userDTO);
            var userCreationResult = await _userManager.CreateAsync(user, userDTO.Password);

            if (!userCreationResult.Succeeded)
            {
                foreach (var error in userCreationResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View(userDTO);
            }

            TempData["redirectUrl"] = redirectUrl;
            TempData["RegistrationOk"] = $"{userDTO.Email} profile created successfully, now you can login.";
            return Redirect(redirectUrl);
        }

        [HttpGet]
        public IActionResult Signin(string? redirectUrl = null)
        {
            TempData["redirectUrl"] = redirectUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signin([FromForm] SignInUserDTO userDTO, string? redirectUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(userDTO);
            }
            
            SignInResult result;
            var user = await _userManager.FindByEmailAsync(userDTO.Email);
            try
            {
                result = await _signInManager.PasswordSignInAsync(user.UserName, userDTO.Password, userDTO.RememberMe, false);
            }
            catch (NullReferenceException e)
            {
                ModelState.AddModelError("", "Invalid email or Password");
                return View(userDTO);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid email or Password");
                return View(userDTO);
            }

            if (redirectUrl != null)
            {
                return Redirect(redirectUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordDTO);
            }

            User currUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var result = await _userManager.ChangePasswordAsync(currUser, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View(changePasswordDTO);
            }
            else
            {
                await _dbContext.SaveChangesAsync();
                TempData["ChangePasswordSuccess"] = "Your password was successfully changed";
                return RedirectToAction("List", "Account", new {id = currUser.Id});
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDTO forgotPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordDTO);
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callback = Url.Action(nameof(ResetPassword), "Auth", new { token, email = user.Email }, Request.Scheme);

            var message = new Message(new string[] { user.Email }, "Reset password | OnlineBalance", callback);
            _emailSender.SendEmail(message);

            TempData["PasswordEmailRessetSentSuccessfully"] = "You successfully requested password reset link, check your email for it, please";
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var resetPasswordDTO = new ResetPasswordDTO { Token = token, Email = email };
            return View(resetPasswordDTO);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordDTO);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                return NotFound();
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.ConfirmPassword);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }

            return RedirectToAction(nameof(Signin));
        }
    }
}
