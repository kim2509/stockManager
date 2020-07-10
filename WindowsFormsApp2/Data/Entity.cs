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
        public string 이전거래량 { get; set; }
        public string diffBefore { get; set; }
        public string upDownRate { get; set; }
        public string realtimeUpDownRate { get; set; }
        public string createdDate { get; set; }
        public string updateDate { get; set; }
        public string 매도호가 { get; set; }
        public string 매수호가 { get; set; }
    }

    public class StockTarget
    {
        public string inqDate { get; set; }
        public string stockCode { get; set; }
        public string stockName { get; set; }
        public string startPrice { get; set; }
        public string currentPrice { get; set; }
        public string targetPrice { get; set; }
        public string status { get; set; }
        public string minsElapsed { get; set; }
        public string buyCnt { get; set; }
        public string sellCnt { get; set; }
        public string waterCnt { get; set; }
        public string 보유수 { get; set; }
        public string 증권사수수료 { get; set; }
        public string 거래세 { get; set; }
        public string 매입단가 { get; set; }
        public string 매입금액 { get; set; }
        public string 평가금액 { get; set; }
        public string 평가손익금액 { get; set; }
        public string 손익률 { get; set; }
    }
    public class StockMyOrder
    {
        public string seq { get; set; }
        public string stockBondGubun { get; set; }
        public string orderNo { get; set; }
        public string stockCode { get; set; }
        public string sellFlag { get; set; }
        public string orderType { get; set; }
        public string Qty { get; set; }
        public string Price { get; set; }
        public string checkQty { get; set; }
        public string reserveDeny{ get; set; }
        public string confirmNo { get; set; }
        public string acceptType { get; set; }
        public string orgOrderNo { get; set; }
        public string stockName { get; set; }
        public string payType { get; set; }
        public string creditTransType { get; set; }
        public string confirmQty { get; set; }
        public string confirmPrice { get; set; }
        public string commType { get; set; }
        public string modifyFlag { get; set; }
        public string confirmedTime { get; set; }
        public string orderDate { get; set; }
    }

    public class StockOrder
    {
        public string Seq { get; set; }
        public string orderNo { get; set; }
        public string inqDate { get; set; }
        public string stockCode { get; set; }
        public string stockName { get; set; }
        public string Qty { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }
        public string OrderType { get; set; }
        public string OrderOption { get; set; }
        public string APIResult { get; set; }
        public string orgOrderNo { get; set; }
        public string createdDate { get; set; }
        public string updateDate { get; set; }
    }

    public class 종목증감정보
    {
        public string 종목코드 { get; set; }
        public string 종목명 { get; set; }
        public double 증가율 { get; set; }
        public int 갯수 { get; set; }
    }
}
