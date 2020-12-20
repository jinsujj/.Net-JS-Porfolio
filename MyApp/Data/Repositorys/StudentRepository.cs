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
    public class StudentRepository : IStudentRepository
    {
        private IConfiguration _config;
        private ILogger<StudentRepository> _logger;
        private MySqlConnection con;

        public StudentRepository(IConfiguration config, ILogger<StudentRepository> logger)
        {
            _config = config;
            _logger = logger;
            con = new MySqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
        }

        public void AddStudent(Student student)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", value: student.Name, dbType: DbType.String);
            parameters.Add("@Age", value: student.Age, dbType: DbType.Int32);
            parameters.Add("@Country", value: student.Country, dbType: DbType.String);

            _logger.LogInformation("데이터 저장");
            try
            {
                string sql = @"INSERT INTO student (Name, Age, Country)
                           VALUES(@Name, @Age, @Country)";

                con.Execute(sql, parameters, commandType: CommandType.Text);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("데이터 저장 에러: +" + ex);
            }
        }

        public IEnumerable<Student> GetAllStudents()
        {
            _logger.LogInformation("데이터 조회(All)");
            try
            {
                string sql = @"SELECT * FROM student";
                return con.Query<Student>(sql).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError("데이터 조회 에러: +" + ex);
                return null;
            }
        }

        public Student GetStudent(int id)
        {
            _logger.LogInformation("데이터 조회(id)");
            try
            {
                var sql = @"SELECT * FROM student WHERE id = @id";
                return con.Query<Student>(sql, new { id = id }, commandType: CommandType.Text).SingleOrDefault();
            }
            catch (System.Exception ex)
            {
                _logger.LogError("데이터 조회 에러: + " + ex);
                return null;
            }

        }

        public void Edit(Student student)
        {
            _logger.LogInformation("데이터 변경(student)");
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", value: student.Id, dbType: DbType.Int32);
                parameters.Add("@Name", value: student.Name, dbType: DbType.String);
                parameters.Add("@Age", value: student.Age, dbType: DbType.Int32);
                parameters.Add("@Country", value: student.Country, dbType: DbType.String);

                string sql = @"UPDATE student
                                SET name = @Name,
                                    age = @Age,
                                    Country = @Country
                                WHERE id = @id";

                con.Execute(sql, parameters, commandType: CommandType.Text);
                    
            }
            catch (System.Exception ex)
            {
                _logger.LogError("데이터 수정 에러 : " + ex);
            }
        }

        public void Delte(Student student)
        {
            _logger.LogInformation("데이터 삭제(student)");
            try
            {
                string sql = @"DELETE FROM student
                               WHERE id = @id";
                con.Execute(sql, new { id = student.Id }, commandType: CommandType.Text);
            }
            catch(System.Exception ex)
            {
                _logger.LogError("데이터 삭제 에러 : " + ex);
            }
        }

        public void Log(string page, string ip)
        {
            con.Execute(@"INSERT INTO log SET page = @page, ip = @ip, date = NOW()"
            , new { page = page, ip = ip });
        }
    }
}
