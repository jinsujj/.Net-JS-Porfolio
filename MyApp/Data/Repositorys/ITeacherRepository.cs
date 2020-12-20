using MyApp.Models;
using System.Collections.Generic;

namespace MyApp.Data.Repositorys
{
    public interface ITeacherRepository
    {
        IEnumerable<Teacher> GetAllTeachers();
        Teacher GetTeacher(int id);
        void Log(string page, string ip);
    }
}