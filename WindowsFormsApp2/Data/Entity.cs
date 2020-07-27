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
        public string 매도방식 { get; set; }
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
        public string ConfirmQty { get; set; }
        public string ConfirmPrice { get; set; }
        public string BuySeq { get; set; }
        public string Status { get; set; }
        public string OrderType { get; set; }
        public string OrderOption { get; set; }
        public string Reason { get; set; }
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

        public double 거래량증가율 { get; set; }
        public double 체결강도증가율 { get; set; }
        public double 총매도잔량증가율 { get; set; }
        public double 총매수잔량증가율 { get; set; }
        public int 갯수 { get; set; }
    }

    public class 종목실시간정보
    {
        public string 종목코드 { get; set; }
        public string 종목명 { get; set; }
        public string 현재가 { get; set; }
        public string 실시간현재가등락률 { get; set; }
        public string 등락률변동률 { get; set; }
        public string 기준가 { get; set; }
        public string 전일대비 { get; set; }
        public string 전일대비기호 { get; set; }
        public string 등락율 { get; set; }
        public string 거래량 { get; set; }
        public string 거래량등락률 { get; set; }
        public string 거래대금 { get; set; }
        public string 체결량 { get; set; }
        public string 체결강도 { get; set; }
        public string 체결강도등락률 { get; set; }
        public string 전일거래량대비 { get; set; }
        public string 매도호가 { get; set; }
        public string 매수호가 { get; set; }
        public string 매도1차호가 { get; set; }
        public string 매도2차호가 { get; set; }
        public string 매도3차호가 { get; set; }
        public string 매도4차호가 { get; set; }
        public string 매도5차호가 { get; set; }
        public string 매수1차호가 { get; set; }
        public string 매수2차호가 { get; set; }
        public string 매수3차호가 { get; set; }
        public string 매수4차호가 { get; set; }
        public string 매수5차호가 { get; set; }
        public string 상한가 { get; set; }
        public string 하한가 { get; set; }
        public string 시가 { get; set; }
        public string 고가 { get; set; }
        public string 저가 { get; set; }
        public string 종가 { get; set; }
        public string 체결시간 { get; set; }
        public string 예상체결가 { get; set; }
        public string 예상체결량 { get; set; }
        public string 자본금 { get; set; }
        public string 액면가 { get; set; }
        public string 시가총액 { get; set; }
        public string 주식수 { get; set; }
        public string 호가시간 { get; set; }
        public string 일자 { get; set; }
        public string 우선매도잔량 { get; set; }
        public string 우선매수잔량 { get; set; }
        public string 우선매도건수 { get; set; }
        public string 우선매수건수 { get; set; }
        public string 총매도잔량 { get; set; }
        public string 총매수잔량 { get; set; }
        public string 총매도잔량등락률 { get; set; }
        public string 총매수잔량등락률 { get; set; }
        public string 잔량비율 { get; set; }
        public string 잔량비율등락률 { get; set; }
        public string 총매도건수 { get; set; }
        public string 총매수건수 { get; set; }
        public string createdDate { get; set; }
    }

    public class 당일실적
    {
        public string 매도방식 { get; set; }
        public string 들어간금액 { get; set; }
        public string 실현손익금액 { get; set; }
        public string 증권사수수료 { get; set; }
        public string 거래세 { get; set; }
        public string 현재까지실제수익 { get; set; }
        public string 보유중평가금액손익 { get; set; }
        public string 실제예상수익 { get; set; }
    }
}
