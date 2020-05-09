using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2.Data
{
    public class StockCode
    {
        public string stockCode;
        public string stockName;
    }

    public class StockDaily
    {
        public string inqDate { get; set; }
        public string stockCode { get; set; }
        public string stockName { get; set; }
        public string currentPrice { get; set; }
        public string traffic { get; set; }
        public string diffBefore { get; set; }
        public string upDownRate { get; set; }
        public string realtimeUpDownRate { get; set; }
        public string createdDate { get; set; }
        public string updateDate { get; set; }
    }
}
