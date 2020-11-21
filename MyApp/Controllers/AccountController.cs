using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApp.Data.Repositorys;
using MyApp.Data.Repositorys.Login;
using MyApp.Models;
using MyApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _enviorment;
        private readonly IUserRepository _userRepository;
        private readonly ILoginFailedRepository _loginFailedRepository;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IWebHostEnvironment environment,
            IUserRepository userRepository,
            ILoginFailedRepository loginFailedRepository,
            ILogger<AccountController> logger
            )
        {
            _enviorment = environment;
            _userRepository = userRepository;
            _loginFailedRepository = loginFailedRepository;
            _logger = logger;
        }

        public IActionResult Register()
        {
            return View();
        }

        //[User][6][5] : 로그인 폼
        [HttpGet]
        [AllowAnonymous] // 인증되지 않은 사용자도 접근 가능
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 로그인 실패 5회 체크
                if (IsLoginFailed(model.Email))
                {
                    ViewBag.IsLoginFailed = true;
                    return View(model);
                }

                if (_userRepository.IsCorrectUser(model.Email, new CommonLibrary.Security().EncryptPassword(model.Password)))
                {
                    var claims = new List<Claim>()
                    {
                        // 로그인 아이디 지정   userid 만 따진다
                        new Claim("Email", model.Email),
                        new Claim(ClaimTypes.NameIdentifier, model.Email),
                        new Claim(ClaimTypes.Name, model.Email), 
                        // 기본 역할 지정, "Role" 기능에 "Users" 값 부여
                        new Claim(ClaimTypes.Role, "UsersInfo") // 추가 정보 기록
                    };

                    var ci = new ClaimsIdentity(claims, "Cookies");

                    var authenticationProperties = new AuthenticationProperties()
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                        IssuedUtc = DateTimeOffset.UtcNow,
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                        , new ClaimsPrincipal(ci), authenticationProperties); // 옵션

                    return LocalRedirect("/Home/Index");
                }
                else
                {
                    ViewBag.WrongPwd = true;
                    _loginFailedRepository.UpdateLoginCount(model.Email);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return Redirect("/Home/Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (phoneNumberCheck(model) == false)
                {
                    ModelState.AddModelError("", "핸드폰에 '-' 하이픈을 넣어 입력해주세요");
                    return View(model);
                }
                if (_userRepository.GetUserByEmail(model.Email) != null)
                {
                    ModelState.AddModelError("", "이미 가입된 사용자입니다.");
                    return View(model);
                }
                else
                {
                    model.Password = new CommonLibrary.Security().EncryptPassword(model.Password);
                    _userRepository.AddUser(model);
                    return RedirectToAction("Login");
                }

            }
            else
            {
                ModelState.AddModelError("", "잘못된 가입 시도입니다.");
                return View(model);
            }
        }

        [Authorize("Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize("Admin")]
        public IActionResult NoteManager()
        {
            return View();
        }

        [Authorize("Admin")]
        public IActionResult UserManager()
        {
            return View();
        }

        public void ClearLoginFailed(string userName)
        {
            _loginFailedRepository.ClearLogin(userName);
        }

        public void IsWrongPassword(string userName)
        {
            // 카운팅 증가
            _loginFailedRepository.UpdateLoginCount(userName);
        }

        public bool IsLoginFailed(string email)
        {
            if (_loginFailedRepository.IsLoginUser(email))
            {
                //로그인 유저
                // 접속 5 이상 && 10분 이내 시도
                if (_loginFailedRepository.IsFiveOverCount(email) && _loginFailedRepository.IsLastLoginWithinTenMinute(email))
                {
                    return true;
                }
                // 접속 5 이하 && 10 분 초과 
                else if (!_loginFailedRepository.IsFiveOverCount(email) && !_loginFailedRepository.IsLastLoginWithinTenMinute(email))
                {
                    _loginFailedRepository.ClearLogin(email);
                    return false; // 로그인 성공
                }
                else
                {
                    return false;  // 로그인 성공, 계정 잠금 
                }
            }
            else
            {
                //첫 로그인
                _loginFailedRepository.AddLogin(new UserLog() { Email = email });
                return false; // 로그인 성공
            }
        }


        private bool phoneNumberCheck(RegisterViewModel model)
        {
            string phone = model.PhoneNumber;
            if (phone.Length == 12 || phone.Length == 13)
            {
                Regex regex = new Regex(@"01{1}[016789]{1}-[0-9]{3,4}-[0-9]{4}");
                Match m = regex.Match(phone);
                if (m.Success) return true;
                else return false;
            }
            else
                return false;
        }
    }
}