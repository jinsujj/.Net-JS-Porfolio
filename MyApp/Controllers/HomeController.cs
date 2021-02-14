using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyApp.Data.Repositorys;
using MyApp.Data.Repositorys.DashBoard;
using MyApp.Models;
using MyApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _enviorment;
        private readonly IDashBoardRepository _repository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IConfiguration config,
            IWebHostEnvironment enviorment,
            IDashBoardRepository repository,
            ILogger<HomeController> logger
            )
        {
            _config = config;
            _enviorment = enviorment;
            _repository = repository;
            _logger = logger;
        }
        /// <summary>
        /// Display 역할
        /// </summary>
        /// 

        [HttpPost]
        public string SendMessage(string name, string email, string subject, string message)
        {
            string bot_mail = _config.GetSection("Email").GetSection("mail").Value;
            string bot_password = _config.GetSection("Email").GetSection("password").Value;
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(email);
                mail.To.Add(new MailAddress("wlstncjs1234@naver.com"));

                mail.Subject = name + ", " + subject;
                mail.Body = "[" + email + "]" + "\n" + message;

                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(bot_mail, bot_password);
                client.Send(mail);

                mail.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return "Your message has been sent. Thank you!";
        }
        public IActionResult Index()
        {
            _logger.LogInformation("Home 페이지 로딩");
            _repository.Log("Home", HttpContext.Connection.RemoteIpAddress.ToString());
            return View();
        }

        public IActionResult Portfolio()
        {
            _logger.LogInformation("portfolio 페이지 로딩");
            _repository.Log("portfolio", HttpContext.Connection.RemoteIpAddress.ToString());
            return Redirect("/index.html");
        }


        /// <summary>
        /// View 에서 넘어온 값을 받는 역할
        /// </summary>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Student(StudentTeacherViewModel model)
        //{
        //    //[ValidateAntiForgeryToken] 교차 사이트 요청 위조 (XSRF/CSRF) 공격 방지

        //    if (ModelState.IsValid)
        //    {
        //        _studentRepository.AddStudent(model.Student);

        //        ModelState.Clear();
        //    }
        //    else
        //    {
        //        // 에러 팝업 출력
        //    }
        //    var students= _studentRepository.GetAllStudents();
        //    var viewModel = new StudentTeacherViewModel()
        //    {
        //        Student = new Student(),
        //        Students = students
        //    };
        //    return View(viewModel);
        //}

    }
}
