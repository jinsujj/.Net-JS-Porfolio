using MyApp.Models.SexualTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Data.Repositorys.SexualTest
{
    public interface ISexualTestRepository
    {
        void InsertResult(ResultLog log);
        void InsertType(String result);
        SexualType GetTypeList();
    }
}
