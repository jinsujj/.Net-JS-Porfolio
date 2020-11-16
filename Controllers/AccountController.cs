﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Login()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "UserId"), // UniqID
                new Claim(ClaimTypes.Name, "UserName"),
                new Claim(ClaimTypes.Email, "UserEmail"),
                new Claim("관리자","장진수")
            };

            // claimsIdentity 1번 방식    [1번 == 2번] 방식
            var claimsIdentity1 = new ClaimsIdentity(claims, "Cookies");
            var clamimsPrincipal1 = new ClaimsPrincipal(claimsIdentity1);

            await HttpContext.SignInAsync(clamimsPrincipal1);

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return Redirect("/Home/Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = new ApplicationUser { UserName = model.FullName, Email = model.Email };

                //if (result.Succeeded)
                //    return RedirectToAction("Login");

                ModelState.AddModelError("", "회원가입 실패");
            }
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = true; // _signInManager.Password
        //        if (result)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        ModelState.AddModelError("", "로그인 실패");
        //    }
        //    return View(model);
        //}

        //public async Task<IActionResult> Logout()
        //{
        //    await _wdwdwd();
        //    return RedirectToAction("Login");
        //}

    }
}