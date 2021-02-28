using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Data.Repositorys.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        private IConfiguration _config;
        private ILogger<CategoryRepository> _logger;
        private MySqlConnection con;

        public CategoryRepository(IConfiguration config, ILogger<CategoryRepository> logger)
        {
            _config = config;
            _logger = logger;
        }

        public List<object> GetCategory()
        {
            _logger.LogInformation("Get Category");
            try
            {
                string sql = @"SELECT * FROM category";
                var result = con.Query<object>(sql).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Category Error");
                List<object> err = new List<object>();
                err.Add("Error");
                return err;
            }
        }
    }
}
