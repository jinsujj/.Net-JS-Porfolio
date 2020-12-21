using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApp.Data.Repositorys.DashBoard;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    [Authorize("Admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _enviorment;
        private readonly IDashBoardRepository _repository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IWebHostEnvironment enviorment,
            IDashBoardRepository repository,
            ILogger<AdminController> logger)
        {
            _enviorment = enviorment;
            _repository = repository;
            _logger = logger;
        }


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
            return Redirect("https://jinsu-blog.iptime.org:4200/");
        }

        /// <summary>
        /// TODO: 사용자 관리자 페이지
        /// </summary>
        public IActionResult UserManager()
        {
            _logger.LogInformation("사용자 DashBoard 로딩");
            List<Log> logs = _repository.GetAllLog();
            return View(logs);
        }
    }
}
