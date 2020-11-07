using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApp.Data.Repositorys;
using MyApp.Models;
using MyApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _enviorment;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IWebHostEnvironment enviorment, 
            ITeacherRepository teacherRepository, 
            IStudentRepository studentRepository,
            ILogger<HomeController> logger
            )
        {
            _enviorment = enviorment;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _logger = logger;
        }
        /// <summary>
        /// Display 역할
        /// </summary>
        /// 
        public IActionResult Index()
        {
            var teachers = _teacherRepository.GetAllTeachers();

            var viewModel = new StudentTeacherViewModel()
            {
                Student = new Student(),
                Teachers = teachers
            };
            return View(viewModel);
        }
        public IActionResult Student()
        {
            var students = _studentRepository.GetAllStudents();

            var viewModel = new StudentTeacherViewModel()
            {
                Student = new Student(),
                Students = students
            };

            return View(viewModel);
        }

        /// <summary>
        /// View 에서 넘어온 값을 받는 역할
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Student(StudentTeacherViewModel model)
        {
            //[ValidateAntiForgeryToken] 교차 사이트 요청 위조 (XSRF/CSRF) 공격 방지

            if (ModelState.IsValid)
            {
                _studentRepository.AddStudent(model.Student);

                ModelState.Clear();
            }
            else
            {
                // 에러 팝업 출력
            }
            var students= _studentRepository.GetAllStudents();
            var viewModel = new StudentTeacherViewModel()
            {
                Student = new Student(),
                Students = students
            };
            return View(viewModel);
        }


        public IActionResult Detail(int id)
        {
            Student student = _studentRepository.GetStudent(id);
            return View(student);
        }

        public IActionResult Edit(int id)
        {
            Student student = _studentRepository.GetStudent(id);
            return View(student);
        }

        /// <summary>
        /// View 에서 넘어온 값을 받는 역할
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student model)
        {
            //[ValidateAntiForgeryToken] 교차 사이트 요청 위조 (XSRF/CSRF) 공격 방지

            if (ModelState.IsValid)
            {
                _studentRepository.Edit(model);

                return RedirectToAction("Student");
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            Student student = _studentRepository.GetStudent(id);
            if(student != null)
            {
                _studentRepository.Delte(student);
            }
            return RedirectToAction("Student");
        }

        //[HttpPost]
        //public IActionResult Student([Bind("Name,Age")] Student model)
        //{
        //    return View();
        //}
    }
}
