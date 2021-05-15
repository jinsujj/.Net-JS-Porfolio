using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyApp.Data.Repositorys.SexualTest;
using MyApp.Models.SexualTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    public class SexualTestController : Controller
    {
        private readonly ISexualTestRepository _repository;
        private readonly ILogger<SexualTestController> _logger;

        public SexualTestController(
            ISexualTestRepository repository,
            ILogger<SexualTestController> logger
            )
        {
            _repository = repository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("SexualTest Intro 로딩");
            return View();
        }

        public IActionResult Loading()
        {
            _logger.LogInformation("SexualTest Loading...");
            return View();
        }

        public IActionResult Test()
        {
            _logger.LogInformation("SexualTest Test 로딩");
            return View();
        }

        public IActionResult ENTP()
        {
            _logger.LogInformation("SexualTest ENTP 로딩");
            return View();
        }

        public IActionResult ESTJ()
        {
            _logger.LogInformation("SexualTest ESTJ 로딩");
            return View();
        }

        public IActionResult INFJ()
        {
            _logger.LogInformation("SexualTest INFJ 로딩");
            return View();
        }

        public IActionResult INTJ()
        {
            _logger.LogInformation("SexualTest INTJ 로딩");
            return View();
        }

        public IActionResult ISFJ()
        {
            _logger.LogInformation("SexualTest ISFJ 로딩");
            return View();
        }

        public IActionResult ISTJ()
        {
            _logger.LogInformation("SexualTest ISTJ 로딩");
            return View();
        }

        public IActionResult Board()
        {
            _logger.LogInformation("SexualTest Board 로딩");
            return View();
        }

        [HttpGet]
        public JsonResult getTypeList()
        {
            var List = _repository.GetTypeList();
            return Json(List);
        }

        [HttpPost]
        public JsonResult result(List<String> res)
        {
            ResultLog log = new ResultLog();
            string result = "";
            int cnt_A = 0;

            log.ip= HttpContext.Connection.RemoteIpAddress.ToString();
            log.date = DateTime.Now.ToString();
            log.sex = res[0];
            log.age = Int32.Parse(res[13]);

            for (int i = 1; i <= 12; i++) {
                log.num[i] = res[i];

                if (res[i] ==  "A") cnt_A += 1;
            }

            // Check Sexual Type
            if(log.num[1] ==  "A" && log.num[3] == "B" && log.num[5] == "B")
            {
                result = "ISTJ";
            }
            else if(log.num[4] ==  "A" && log.num[7] ==  "A" && log.num[8] == "B" && log.num[10] == "B")
            {
                result = "ISFJ";
            }
            else if (log.num[1] == "B" && log.num[4] == "B" && log.num[7] == "B" && log.num[11] == "B")
            {
                result = "INTJ";
            }
            else if(log.num[5] ==  "A" && log.num[6] =="B" && log.num[7] ==  "A" && log.num[12] ==  "A")
            {
                result = "INFJ";
            }
            else if(log.num[1] ==  "A" && log.num[4] == "B" && log.num[8] == "B" && log.num[11] ==  "A")
            {
                result = "ESTJ";
            }
            else if(log.num[1] ==  "A" && log.num[4] == "B" && log.num[7] =="B" && log.num[8] ==  "A")
            {
                result = "ENTP";
            }

            if(result == "")
            {
                if (cnt_A <= 2) result = "ISTJ";
                else if (cnt_A <= 4) result = "ISFJ";
                else if (cnt_A <= 6) result = "INFJ";
                else if (cnt_A <= 8) result = "INTJ";
                else if (cnt_A <= 10) result = "ESTJ";
                else if (cnt_A <= 12) result = "ENTP";
            }
            log.type = result;

            // Save Result Log
            _repository.InsertResult(log);

            // Save Result Type 
            _repository.InsertType(result);

            return Json(result);
        }

    }
}
