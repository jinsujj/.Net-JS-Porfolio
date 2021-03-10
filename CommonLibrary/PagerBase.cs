using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public class PagerBase
    {
        public string Category { get; set; }
        public string Url { get; set; }
        public int PageCount { get; set; } = 5;
        public int RecordCount { get; set; } = 50;
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 0;
        public int PageNumber { get; set; } = 1;
        public int PagerButtonCount { get; set; } = 5;
        public bool SearchMode { get; set; } = false;
        public string SearchField { get; set; } = "";
        public string SearchQuery { get; set; } = "";
    }
}
