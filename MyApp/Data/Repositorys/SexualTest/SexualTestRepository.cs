using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyApp.Models.SexualTest;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;

namespace MyApp.Data.Repositorys.SexualTest
{
    public class SexualTestRepository : ISexualTestRepository
    {
        private IConfiguration _config;
        private MySqlConnection con;
        private ILogger<SexualTestRepository> _logger;

        public SexualTestRepository(IConfiguration config, ILogger<SexualTestRepository> logger)
        {
            _config = config;
            con = new MySqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            _logger = logger;
        }

        public void InsertResult(ResultLog log)
        {
            _logger.LogInformation("Result Log" + DateTime.Now);
            try
            {
                 con.Execute(@"INSERT INTO SexualLog (ip, date, age, sex, type, num1, num2, num3, num4, num5, num6, num7 ,num8, num9 ,num10, num11, mum12)
                            VALUES (@ip, @date, @age, @sex, @num1, @num2, @num3, @num4, @num5, @num6, @num7, @num8, @num9, @num10, @num11, @num12)"
                    , new { ip = log.ip, date = log.date, age = log.age, sex = log.sex, type = log.type,
                        num1 = log.num[1], num2 = log.num[2], num3 = log.num[3], num4 = log.num[4],
                        num5 = log.num[5], num6 = log.num[6], num7 = log.num[7], num8 = log.num[8],
                        num9 = log.num[9], num10 = log.num[10], num11 = log.num[11], num12 = log.num[12]
                        });
                return;
            }
            catch(Exception ex)
            {
                _logger.LogError("Resul log Insert 에러" + ex);
                return;
            }
        }

        public void InsertType(string type)
        {
            _logger.LogInformation("InsertType");
            try
            {
                if(type == "ENTP")
                {
                    con.Execute(@"UPDATE SexualType SET ENTP = ENTP +1");
                }
                else if(type == "ESTJ")
                {
                    con.Execute(@"UPDATE SexualType SET ESTJ = ESTJ +1");
                }
                else if(type == "INFJ")
                {
                    con.Execute(@"UPDATE SexualType SET INFJ = INFJ +1");
                }
                else if(type == "INTJ")
                {
                    con.Execute(@"UPDATE SexualType SET INTJ = INTJ +1");
                }
                else if(type == "ISFJ")
                {
                    con.Execute(@"UPDATE SexualType SET ISFJ = ISFJ +1");
                }
                else if(type == "ISTJ")
                {
                    con.Execute(@"UPDATE SexualType SET ISTJ = ISTJ +1");
                }
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError("Insert Type 에러" + ex);
                return;
            }
        }

        public SexualType GetTypeList()
        {
            _logger.LogInformation("Get Type List");
            try
            {
                string sql = @"SELECT * FROM SexualType";
                return con.Query<SexualType>(sql, commandType: CommandType.Text).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Type List Error");
                return null;
            }
        }
    }
}
