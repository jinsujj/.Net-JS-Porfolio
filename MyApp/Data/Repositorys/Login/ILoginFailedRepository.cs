using MyApp.Models;

namespace MyApp.Data.Repositorys.Login
{
    public interface ILoginFailedRepository
    {
        UserLog AddLogin(UserLog model);
        void ClearLogin(string email);
        bool IsFiveOverCount(string email);
        bool IsLastLoginWithinTenMinute(string email);
        bool IsLoginUser(string email);
        void UpdateLoginCount(string email);
    }
}