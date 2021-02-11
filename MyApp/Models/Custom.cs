using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Models
{
    public class Custom
    {
        public string buffquery { get; set; }
        private string query;
        private List<string> columnList;
        private List<List<string>> dataRaw;

        public Custom()
        {
            this.columnList = new List<string>();
            this.dataRaw = new List<List<string>>();
        }

        public string getQuery()
        {
            return query;
        }

        public void setQuery(string str)
        {
            query = str;
        }

        public void setColumn(string str)
        {
            columnList.Add(str);
        }
        public List<string> getColumnList()
        {
            return columnList;
        }

        public void setRaw(List<String> listStr)
        {
            // 동일한 값 주소를 Add 하면 마지막 Add 값으로 전부 복사되어 변경
            List<string> buff = listStr;
            dataRaw.Add(buff.ToList());
        }
        
        public List<List<string>> getRaw()
        {
            return dataRaw;
        }
        
    }
}
