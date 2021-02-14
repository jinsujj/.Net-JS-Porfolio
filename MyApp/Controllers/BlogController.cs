using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyApp.Data.Repositorys.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    public class BlogController :Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<BlogController> _logger;
        private readonly IDashBoardRepository _repository;

        public BlogController(IConfiguration config, ILogger<BlogController> logger, IDashBoardRepository repository)
        {
            _config = config;
            _logger = logger;
            _repository = repository;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Blog -> Home 페이지 로딩");
            _repository.Log("Blog", HttpContext.Connection.RemoteIpAddress.ToString());
            return Redirect("../DotNetNote/IndexCard");
        }

    }
}
