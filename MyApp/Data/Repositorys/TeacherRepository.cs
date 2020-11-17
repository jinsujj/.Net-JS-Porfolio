using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyApp.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Data.Repositorys
{
    public class TeacherRepository : ITeacherRepository
    {
        private IConfiguration _config;
        private MySqlConnection con;
        private ILogger<TeacherRepository> _logger;

        public TeacherRepository(IConfiguration config, ILogger<TeacherRepository> logger)
        {
            _config = config;
            _logger = logger;
            con = new MySqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
        }

        public IEnumerable<Teacher> GetAllTeachers()
        {
            _logger.LogInformation("데이터 조회(All)");
            try
            {
                string sql = @"SELECT * FROM teacher";
                var result = con.Query<Teacher>(sql).ToList();
                return result;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("데이터 조회 에러: +" + ex);
                return null;
            }
        }

        public Teacher GetTeacher(int id)
        {
            _logger.LogInformation("데이터 조회(id)");
            try
            {
                return con.Query<Teacher>(@"SELECT * FROM teacher WHERE id = @id"
                , new { id = id }
                , commandType: CommandType.Text).SingleOrDefault();
            }
            catch (System.Exception ex)
            {
                _logger.LogError("데이터 조회 에러: +" + ex);
                return null;
            }
        }
    }
}
