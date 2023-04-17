using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_platform.Models;
using CI_platform.Repositories.GenericRepository.Interface;
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
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }


        //---------------------- FOR REGISTARATION --------------------------//

        #region Registration
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
            var obj = _unitOfWork.User.GetFirstOrDefault(m => m.Email == _userRegisterModel.Email);

            if (obj == null)
            {
                _unitOfWork.User.Add(user);
                _unitOfWork.Save();
                return RedirectToAction("login", "Home");
            }
            else
            {
                ViewBag.RegError = "email already exist";
            }

            return View();
        }
        #endregion

        //---------------------- FOR LOGIN --------------------------//

        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public IActionResult Login(userLoginModel obj)
        {
            var status = _unitOfWork.User.GetFirstOrDefault(m => m.Email == obj.Email && m.Password == obj.Password);
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
        #endregion

        //---------------------- FOR LOGOUT --------------------------//

        #region Logout
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("platformLandingPage", "Pages");
        }
        #endregion

        //---------------------- FOR FORGOT PASSWORD --------------------------//

        #region Forgot Password
        [HttpGet]
        public IActionResult forgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult forgotPassword(userForgotPasswordModel _userForgotPasswordModel)
        {

            var status = _unitOfWork.User.GetFirstOrDefault(m => m.Email == _userForgotPasswordModel.Email);
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
            _unitOfWork.PasswordReset.Add(passwordReset);
            _unitOfWork.Save();

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
                Credentials = new NetworkCredential("kanjiyashyam15@gmail.com", "mbxfpwtiaztubcng"),
                EnableSsl = true
            };
            smtpClient.Send(message);

            return RedirectToAction("login", "Home");
        }
        #endregion

        //---------------------- FOR CHANGE PASSWORD --------------------------//

        #region Change Password
        [HttpGet]
        [AllowAnonymous]
        public IActionResult changePassword(string Email, string Token)
        {
            var changePassword = _unitOfWork.PasswordReset.GetFirstOrDefault(x => x.Email == Email && x.Token == Token);

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
            var status = _unitOfWork.User.GetFirstOrDefault(x => x.Email == _userChangePasswordModel.Email);
            if (status == null)
            {
                return RedirectToAction("forgetPassword", "Home");
            }

            status.Password = _userChangePasswordModel.Password;
            _unitOfWork.User.Update(status);
            _unitOfWork.Save();

            var deletion = _unitOfWork.PasswordReset.GetFirstOrDefault(x => x.Token == _userChangePasswordModel.Token && x.Email == _userChangePasswordModel.Email);
            if (deletion != null)
            {
                _unitOfWork.PasswordReset.Remove(deletion);
            }

            return RedirectToAction("login", "Home");
        }
        #endregion

        //---------------------------- Hashing --------------------------------//

        #region Hashing
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
        #endregion

        //---------------------------- Error --------------------------------//

        #region Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

    }

}