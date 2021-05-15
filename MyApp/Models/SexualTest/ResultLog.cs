using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Models.SexualTest
{
    public class ResultLog
    {
        public string ip { get; set; }
        public string date { get; set; }
        public int age { get; set; }
        public string sex { get; set; }
        public string type { get; set; }
        public string[] num { get; set; } = new string[13];
    }
}
