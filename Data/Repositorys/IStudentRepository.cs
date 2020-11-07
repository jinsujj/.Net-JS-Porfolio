using MyApp.Models;
using System.Collections.Generic;

namespace MyApp.Data.Repositorys
{
    public interface IStudentRepository
    {
        void AddStudent(Student student);
        IEnumerable<Student> GetAllStudents();
        Student GetStudent(int id);
        void Edit(Student student);
        void Delte(Student student);
    }
}