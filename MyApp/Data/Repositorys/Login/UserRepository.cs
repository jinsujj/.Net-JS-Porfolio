﻿using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyApp.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Data.Repositorys.Login
{
    public class UserRepository : IUserRepository
    {
        private IConfiguration _config;
        private ILogger<UserRepository> _logger;
        private MySqlConnection con;

        public UserRepository(IConfiguration config, ILogger<UserRepository> logger)
        {
            _config = config;
            _logger = logger;
            con = new MySqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
        }

        public void AddUser(RegisterViewModel model)
        {
            _logger.LogInformation("유저 등록");
            try
            {
                string sql = @"INSERT INTO user (email, fullname, phone_number, password)
                               VALUES ( @email , @fullname, @phone_number, @password);";

                con.Execute(sql, new { email = model.Email, fullname = model.FullName, phone_number = model.PhoneNumber, password = model.Password }
                    , commandType: CommandType.Text);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("유저 등록 에러: +" + ex);
                return;
            }
        }

        public RegisterViewModel GetUserByEmail(string email)
        {
            _logger.LogInformation("유저 조회(id)");
            try
            {
                string sql = @"SELECT * FROM user WHERE email = @email";
                return con.Query<RegisterViewModel>(sql, new { email= email }, commandType: CommandType.Text).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError("유저 조회(id) 에러: +" + ex);
                return null;
            }
        }

        public bool IsCorrectUser(string email, string password)
        {
            _logger.LogInformation("유저 확인");
            bool result = false;
            try
            {
                string sql = @"SELECT * FROM user
                               WHERE email = @email
                               AND password = @password";

                var isExist = con.Query(sql, new { email = email, password = password }, commandType: CommandType.Text).SingleOrDefault();
                if (isExist != null)
                {
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("유저 확인 에러: +" + ex);
                return result;
            }
        }


        public void ModifyUser(RegisterViewModel model)
        {
            _logger.LogInformation("유저 정보 변경");
            try
            {
                string sql = @"UPDATE user SET 
                                email = @email, 
                                fullname = @fullname, 
                                phone_number = @phone_number,
                                password = @password ";
                con.Execute(sql, new { email = model.Email, fullname = model.FullName, phone_number = model.PhoneNumber, password = model.Password }
                        , commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                _logger.LogError("유저 정보 변경 에러: +" + ex);
            }
        }
    }
}