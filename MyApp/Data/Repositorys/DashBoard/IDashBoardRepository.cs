using MyApp.Models;
using System;
using System.Collections.Generic;

namespace MyApp.Data.Repositorys.DashBoard
{
    public interface IDashBoardRepository
    {
        List<Log> GetAllLog();
        List<Log> GetLog(DateTime from, DateTime to);
    }
}