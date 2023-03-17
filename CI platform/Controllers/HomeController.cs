using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_platform.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace CI_platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CIDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, CIDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }


        //---------------------- FOR REGISTARATION --------------------------//

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(userRegisterModel _userRegisterModel)
        {
            var user = new User
            {
                FirstName = _userRegisterModel.FirstName,
                LastName = _userRegisterModel.LastName,
                PhoneNumber = _userRegisterModel.PhoneNumber,
                Email = _userRegisterModel.Email,
                Password = _userRegisterModel.Password,
                CityId = 8,
                CountryId = 1,

            };
            var obj = _dbContext.Users.FirstOrDefault(m => m.Email == _userRegisterModel.Email);

            if (obj == null)
            {
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return RedirectToAction("login", "Home");
            }
            else
            {
                ViewBag.RegError = "email already exist";
            }

            return View();
        }
        //-----------------------------------------------------------//

        //---------------------- FOR LOGIN --------------------------//
        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public IActionResult Login(userLoginModel obj)
        {
            var status = _dbContext.Users.FirstOrDefault(m => m.Email == obj.Email && m.Password == obj.Password);
            if (status == null)
            {
                ViewBag.LoginStatus = 0;
            }
            else
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, status.Email) },
                CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, status.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, status.LastName));
                identity.AddClaim(new Claim(ClaimTypes.Email, status.Email));
                var principle = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
                HttpContext.Session.SetString("Email", status.Email);

                return RedirectToAction("platformLandingPage", "Pages");
            }
            return View(obj);
        }
        //----------------------------------------------------------//


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("platformLandingPage", "Pages");
        }


        //---------------------- FOR FORGOT PASSWORD --------------------------//
        [HttpGet]
        public IActionResult forgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult forgotPassword(userForgotPasswordModel _userForgotPasswordModel)
        {

            var status = _dbContext.Users.FirstOrDefault(m => m.Email == _userForgotPasswordModel.Email);
            if (status == null)
            {
                return RedirectToAction("login", "Home");
            }

            var token = Guid.NewGuid().ToString();

            var passwordReset = new Entities.DataModels.PasswordReset
            {
                Email = _userForgotPasswordModel.Email,
                Token = token,
            };
            _dbContext.Add(passwordReset);
            _dbContext.SaveChanges();

            var resetLink = Url.Action("changePassword", "Home", new { email = _userForgotPasswordModel.Email, token }, Request.Scheme);

            var fromAddress = new MailAddress("kanjiyashyam15@gmail.com", "Shyam Kanjiya");
            var toAddress = new MailAddress(_userForgotPasswordModel.Email);
            var subject = "Password reset request";
            var body = $"Hi,<br /><br />Please click on the following link to reset your password:<br /><br /><a href='{resetLink}'>{resetLink}</a>";

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("kanjiyashyam15@gmail.com", "fbxinllsiaplwthg"),
                EnableSsl = true
            };
            smtpClient.Send(message);

            return RedirectToAction("login", "Home");
        }
        //---------------------------------------------------------------------//

        //---------------------- FOR CHANGE PASSWORD --------------------------//
        [HttpGet]
        [AllowAnonymous]
        public IActionResult changePassword(string Email, string Token)
        {
            var changePassword = _dbContext.PasswordResets.FirstOrDefault(x => x.Email == Email && x.Token == Token);

            if (changePassword == null)
            {
                return RedirectToAction("login", "Home");
            }

            var changePasswordModel = new userChangePasswordModel
            {
                Email = Email,
                Token = Token
            };

            return View(changePasswordModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult changePassword(userChangePasswordModel _userChangePasswordModel)
        {
            var status = _dbContext.Users.FirstOrDefault(x => x.Email == _userChangePasswordModel.Email);
            if (status == null)
            {
                return RedirectToAction("forgetPassword", "Home");
            }

            status.Password = _userChangePasswordModel.Password;
            _dbContext.Users.Update(status);
            _dbContext.SaveChanges();

            var deletion = _dbContext.PasswordResets.FirstOrDefault(x => x.Token == _userChangePasswordModel.Token && x.Email == _userChangePasswordModel.Email);
            if (deletion != null)
            {
                _dbContext.PasswordResets.Remove(deletion);
            }

            return RedirectToAction("login", "Home");
        }

        //---------------------------------------------------------------------//


        //---------------------------- Hashing --------------------------------//

        /*public static string GetHash(string text)
        {
            // SHA512 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                // Get the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }*/

        //---------------------------------------------------------------------//

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

}