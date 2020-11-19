using Dapper;
using Microsoft.Extensions.Configuration;
using MyApp.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Data.Repositorys.Login
{
    public class LoginFailedRepository : ILoginFailedRepository
    {
        private IConfiguration _config;
        private MySqlConnection db;

        public LoginFailedRepository(IConfiguration config)
        {
            _config = config;
            db = new MySqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
        }

        public UserLog AddLogin(UserLog model)
        {
            string sql = @"INSERT INTO userlogs (email) VALUES (@email);
                           SELECT * FROM userlogs WHERE email = @email";
            var id = db.Query<int>(sql, model).Single();
            model.Id = id;
            return model;
        }

        public void ClearLogin(string email)
        {
            string sql = @"UPDATE userlogs 
                            SET FailedPasswordAttemptCount =0 , FailedPasswordAttemptTime = NOW()
                          WHERE email = @email";
            db.Execute(sql, new { email = email });
        }

        public bool IsFiveOverCount(string email)
        {
            string sql = @"SELECT FailedPasswordAttemptCount 
                           FROM userlogs
                           WHERE email = @email";
            int r = db.Query<int>(sql, new { email = email }).SingleOrDefault();
            if (r >= 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLastLoginWithinTenMinute(string email)
        {
            string sql = @"SELECT TIMESTAMPDIFF(MINUTE, FailedPasswordAttemptTime, NOW())
                           FROM userlogs
                           WHERE email = @email";
            var r = db.Query<int>(sql, new { email = email }).SingleOrDefault();
            if (r <= 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLoginUser(string email)
        {
            string sql = @"SELECT COUNT(*) 
                           FROM userlogs 
                           WHERE email = @email";
            int cnt = db.Query<int>(sql, new { email = email }).Single();

            if (cnt > 0)
            {
                return true; // 기존에 로그인한 유저
            }
            else
            {
                return false;
            }
        }

        public void UpdateLoginCount(string email)
        {
            string sql = @"UPDATE userlogs
                           SET FailedPasswordAttemptCount = FailedPasswordAttemptCount +1 , FailedPasswordAttemptTime = NOW()
                           WHERE email = @email";
            db.Execute(sql, new { email = email });
        }

    }
}
