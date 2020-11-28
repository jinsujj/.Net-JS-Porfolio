using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    [Authorize("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// TODO: 게시판 관리자 페이지
        /// </summary>
        public IActionResult NoteManager()
        {
            return View();
        }

        public IActionResult RaspServer()
        {
            return Redirect("https://jinsu-seoul.iptime.org:4200/");
        }

        /// <summary>
        /// TODO: 사용자 관리자 페이지
        /// </summary>
        public IActionResult UserManager()
        {
            return View();
        }
    }
}
