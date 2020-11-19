﻿using MyApp.ViewModels;

namespace MyApp.Data.Repositorys.Login
{
    public interface IUserRepository
    {
        void AddUser(RegisterViewModel model);
        RegisterViewModel GetUserByEmail(string userId);
        bool IsCorrectUser(string email, string password);
        void ModifyUser(RegisterViewModel model);
    }
}