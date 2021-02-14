using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public BlogController(IConfiguration config, ILogger<BlogController> logger)
        {
            _config = config;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Blog -> Home 페이지 로딩");
            return Redirect("../DotNetNote/IndexCard");
        }

    }
}
