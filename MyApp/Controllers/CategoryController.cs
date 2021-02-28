using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApp.Data.Repositorys.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    public class CategoryController: Controller
    {
        private readonly IWebHostEnvironment _enviorment;
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepository _repository;
    }
}
