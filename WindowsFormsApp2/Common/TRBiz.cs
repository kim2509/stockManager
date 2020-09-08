using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp2.Dac;
using WindowsFormsApp2.Data;

namespace WindowsFormsApp2.Common
{
    public class TRBiz
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public APIManager OpenAPI { get; set; }
        string inqDate = DateTime.Now.ToString("yyyyMMdd");
        DacStock dacStock = new DacStock();

        public TRBiz(APIManager api)
        {
            OpenAPI = api;
        }

        public void 거래량순조회()
        {
            OpenAPI.SetInputValue("시장구분", "000"); // 000 : 전체, 001: 코스피, 101 : 코스닥
            OpenAPI.SetInputValue("주기구분", "5");     // 주기구분 = 5:5일, 10:10일, 20:20일, 60:60일, 250:250일
            OpenAPI.SetInputValue("거래량구분", "1000");     // 5:5천주이상, 10:만주이상, 50:5만주이상, 100:10만주이상, 200:20만주이상, 300:30만주이상, 500:50만주이상, 1000:백만주이상
            OpenAPI.CommRqData("거래량순조회", "opt10024", 0, "5001");
        }

        public void 거래량순리스트조회처리(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            int rowCount = OpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (int i = 0; i < rowCount; i++)
            {
                StockDaily stockInfo = new StockDaily();
                stockInfo.inqDate = inqDate;
                stockInfo.stockCode = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                stockInfo.stockName = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();

                stockInfo.currentPrice = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                if (!string.IsNullOrWhiteSpace(stockInfo.currentPrice))
                    stockInfo.currentPrice = Math.Abs(decimal.Parse(stockInfo.currentPrice)).ToString();

                stockInfo.traffic = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "현재거래량").Trim();
                stockInfo.이전거래량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "이전거래량").Trim();
                stockInfo.diffBefore = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일대비").Trim();
                stockInfo.upDownRate = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "등락률").Trim();
                stockInfo.매도호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매도호가").Trim();
                stockInfo.매수호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매수호가").Trim();

                if (dacStock.GetStockDailyInfo(inqDate, stockInfo.stockCode) == null)
                    dacStock.insertStockDaily(stockInfo);
                else
                    dacStock.현재가갱신(inqDate, stockInfo.stockCode, stockInfo.currentPrice);
            }

            // 매수대상리스트에 현재가 갱신
            dacStock.거래량정보에서현재가갱신(inqDate);
        }


        public int 계좌주문별체결현황요청(string sPrevNext = "0")
        {
            if (string.IsNullOrWhiteSpace(Biz.AccountNo))
            {
                return -1;
            }

            // 주문일자 = YYYYMMDD (20170101 연도4자리, 월 2자리, 일 2자리 형식)
            OpenAPI.SetInputValue("주문일자", inqDate);

            // 계좌번호 = 전문 조회할 보유계좌번호
            OpenAPI.SetInputValue("계좌번호", Biz.AccountNo);

            //비밀번호 = 사용안함(공백)
            OpenAPI.SetInputValue("비밀번호", "");

            // 비밀번호입력매체구분 = 00
            OpenAPI.SetInputValue("비밀번호입력매체구분", "00");

            // 주식채권구분 = 0:전체, 1:주식, 2:채권
            OpenAPI.SetInputValue("주식채권구분", "0");

            // 시장구분 = 0:전체, 1:장내, 2:코스닥, 3:OTCBB, 4:ECN
            OpenAPI.SetInputValue("시장구분", "0");

            // 매도수구분 = 0:전체, 1:매도, 2:매수
            OpenAPI.SetInputValue("매도수구분", "0");

            // 조회구분 = 0:전체, 1:체결
            OpenAPI.SetInputValue("조회구분", "0");

            OpenAPI.SetInputValue("종목코드", "");

            string 마지막주문번호 = string.Empty;
            StockMyOrder lastOrder = dacStock.동기화된마지막주문조회(inqDate);
            if (lastOrder != null)
                마지막주문번호 = lastOrder.orderNo;

            log.Info("계좌별주문체결현황요청 마지막주문번호 : " + 마지막주문번호);

            OpenAPI.SetInputValue("시작주문번호", 마지막주문번호);

            return OpenAPI.CommRqData("계좌별주문체결현황요청", "opw00009", !"2".Equals(sPrevNext) ? 0 : 2, "5678");
        }

        public void 계좌별주문체결현황요청응답처리(string orderDate, object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e, string sPrevNext)
        {
            log.Info("계좌별주문체결현황요청응답처리 start orderDate:" + orderDate + " sPrevNext:" + sPrevNext);

            int rowCount = OpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (int i = 0; i < rowCount; i++)
            {
                StockMyOrder myOrder = new StockMyOrder();
                myOrder.orderDate = orderDate;

                myOrder.stockBondGubun = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주식채권구분").Trim();
                myOrder.orderNo = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주문번호").Trim();
                myOrder.stockCode = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목번호").Trim();

                if (myOrder.stockCode.StartsWith("A"))
                    myOrder.stockCode = myOrder.stockCode.Substring(1);
                else if (myOrder.stockCode.StartsWith("*A"))
                    myOrder.stockCode = myOrder.stockCode.Substring(2);

                myOrder.sellFlag = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매매구분").Trim();
                myOrder.orderType = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주문유형구분").Trim();
                myOrder.Qty = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주문수량").Trim();
                myOrder.Price = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주문단가").Trim();
                myOrder.checkQty = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "확인수량").Trim();
                myOrder.reserveDeny = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "예약반대").Trim();
                myOrder.confirmNo = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "체결번호").Trim();
                myOrder.acceptType = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "접수구분").Trim();
                myOrder.orgOrderNo = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "원주문번호").Trim();
                myOrder.stockName = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                myOrder.payType = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "결제구분").Trim();
                myOrder.creditTransType = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "신용거래구분").Trim();
                myOrder.confirmQty = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "체결수량").Trim();
                myOrder.confirmPrice = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "체결단가").Trim();
                myOrder.commType = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "통신구분").Trim();
                myOrder.modifyFlag = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "정정취소구분").Trim();
                myOrder.confirmedTime = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "체결시간").Trim();

                log.Info("체결현황요청응답 : " + JsonConvert.SerializeObject(myOrder));

                if (!string.IsNullOrWhiteSpace(myOrder.orderNo))
                {
                    if (dacStock.체결내역있는지검사(myOrder) == null)
                    {
                        log.Info("신규등록");
                        dacStock.체결내역한건등록(myOrder);
                    }
                    else
                    {
                        log.Info("기존거 수정");
                        dacStock.체결내역업데이트(myOrder);
                    }
                }

            }

            if ("2".Equals(sPrevNext))
            {
                계좌주문별체결현황요청(sPrevNext);
            }

            log.Info("계좌별주문체결현황요청응답처리 start");
        }

        DateTime 최종실시간미체결요청시간 = DateTime.MinValue;

        public int 실시간미체결요청()
        {
            if (string.IsNullOrWhiteSpace(Biz.AccountNo)) return -1;

            if (최종실시간미체결요청시간 == DateTime.MinValue )
            {
                최종실시간미체결요청시간 = DateTime.Now;
            } else if ( DateTime.Now.AddSeconds(-8) <= 최종실시간미체결요청시간 )
            {
                return -1;
            }
            else
            {
                최종실시간미체결요청시간 = DateTime.Now;
            }

            // 계좌번호 = 전문 조회할 보유계좌번호
            OpenAPI.SetInputValue("계좌번호", Biz.AccountNo);

            // 전체종목구분 = 0:전체, 1:종목
            OpenAPI.SetInputValue("전체종목구분", "0");

            // 매매구분 = 0:전체, 1:매도, 2:매수
            OpenAPI.SetInputValue("매매구분", "1");

            OpenAPI.SetInputValue("종목코드", "");

            // 체결구분 = 0:전체, 2:체결, 1:미체결
            OpenAPI.SetInputValue("체결구분", "1");

            return OpenAPI.CommRqData("실시간미체결요청", "opt10075", 0, "1234");
        }

        public void 실시간미체결요청응답처리(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            log.Info("실시간미체결요청응답처리 start");
            int rowCount = OpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (int i = 0; i < rowCount; i++)
            {
                string stockCode = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();

                if (stockCode.StartsWith("A"))
                    stockCode = stockCode.Substring(1);
                else if (stockCode.StartsWith("*A"))
                    stockCode = stockCode.Substring(2);

                string sQty = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주문수량").Trim();
                int qty = int.Parse(sQty);

                string sPrice = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주문가격").Trim();
                int price = int.Parse(sPrice);

                string orderNo = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주문번호").Trim();
                string orderStatus = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주문상태").Trim();

                log.Info("orderNo:" + orderNo + " orderStatus:" + orderStatus + " stockCode:" + stockCode + " sQty:" + sQty + " sPrice:" + sPrice);

                List<StockOrder> orderList = dacStock.tbl_stock_order_주문조회(inqDate, stockCode, "매도", "요청중");

                // 주문번호가 없는 매도요청건들 주문번호 업데이트
                for (int j = 0; j < orderList.Count; j++)
                {
                    if (string.IsNullOrWhiteSpace(orderList[j].orderNo))
                    {
                        int orderQty = int.Parse(orderList[j].Qty);
                        int orderPrice = int.Parse(orderList[j].Price);

                        if (qty == orderQty && price == orderPrice)
                        {
                            log.Info("주문번호업데이트  orderSeq: " + orderList[j].Seq + " orderNo:" + orderNo);
                            dacStock.주문정보업데이트(orderList[j].Seq, "", orderNo);
                        }
                    }
                }
            }

            log.Info("실시간미체결요청응답처리 end");
        }

        public void 당일거래량상위요청(string flag, string 장운영구분,string sPrevNext = "0")
        {
            OpenAPI.SetInputValue("시장구분", "001"); // 000 : 전체, 001: 코스피, 101 : 코스닥

            //정렬구분 = 1:거래량, 2:거래회전율, 3:거래대금
            OpenAPI.SetInputValue("정렬구분", flag);

            // 관리종목포함 = 0:관리종목 포함, 1:관리종목 미포함, 3:우선주제외, 11:정리매매종목제외, 4:관리종목, 우선주제외
            // 5:증100제외, 6:증100마나보기, 13:증60만보기, 12:증50만보기, 7:증40만보기, 8:증30만보기, 9:증20만보기, 14:ETF제외, 15:스팩제외, 16:ETF+ETN제외
            OpenAPI.SetInputValue("관리종목포함", "0");

            // 신용구분 = 0:전체조회, 9:신용융자전체, 1:신용융자A군, 2:신용융자B군, 3:신용융자C군, 4:신용융자D군, 8:신용대주
            OpenAPI.SetInputValue("신용구분", "0");

            // 5:5천주이상, 10:만주이상, 50:5만주이상, 100:10만주이상, 200:20만주이상, 300:30만주이상, 500:50만주이상, 1000:백만주이상
            OpenAPI.SetInputValue("거래량구분", "0");

            // 가격구분 = 0:전체조회, 1:1천원미만, 2:1천원이상, 3:1천원~2천원, 4:2천원~5천원, 5:5천원이상, 6:5천원~1만원, 10:1만원미만, 7:1만원이상, 8:5만원이상, 9:10만원이상
            OpenAPI.SetInputValue("가격구분", "0");

            // 거래대금구분 = 0:전체조회, 1:1천만원이상, 3:3천만원이상, 4:5천만원이상, 10:1억원이상, 30:3억원이상, 50:5억원이상, 100:10억원이상, 300:30억원이상, 500:50억원이상, 1000:100억원이상, 3000:300억원이상, 5000:500억원이상
            OpenAPI.SetInputValue("개래대금구분", "0");

            // 장운영구분 = 0:전체조회, 1:장중, 999:시간외전체, 2:장전시간외, 3:장후시간외
            OpenAPI.SetInputValue("장운영구분", 장운영구분);

            OpenAPI.CommRqData(flag.Equals("1") ? "당일거래량순조회" : "당일거래대금순조회", "opt10030", !"2".Equals(sPrevNext) ? 0 : 2, 장운영구분);
            Thread.Sleep(500);
        }

        public void 당일거래량상위응답처리(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            int rowCount = OpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);
            string 장운영구분 = e.sScrNo;

            log.Info("당일거래량상위응답처리[" + e.sRQName + "] start e.sPrevNext:" + e.sPrevNext);

            for (int i = 0; i < rowCount; i++)
            {
                string 종목코드 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                string 종목명 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                string 현재가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                현재가 = 현재가.Replace("-", "").Replace("+", "");

                string 전일대비기호 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일대비기호").Trim();
                string 전일대비 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일대비").Trim();
                string 등락률 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "등락률").Trim();
                string 거래량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "거래량").Trim();
                string 전일비 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일비").Trim();
                string 거래회전율 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "거래회전율").Trim();
                string 거래금액 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "거래금액").Trim();

                log.Info("종목코드:" + 종목코드 + " 종목명:" + 종목명 + " 현재가:" + 현재가 + " 전일대비기호:" + 전일대비기호 +
                    " 전일대비:" + 전일대비 + " 등락률:" + 등락률 + " 거래량:" + 거래량 + " 전일비:" + 전일비 +
                    " 거래회전율:" + 거래회전율 + " 거래금액:" + 거래금액 + " 장운영구분:" + 장운영구분);

                dacStock.당일장후시간외_거래량거래대금순조회(inqDate, 종목코드, 종목명, 현재가, 전일대비기호, 전일대비
                    , 등락률, 거래량, 전일비, 거래회전율, 거래금액, "당일거래량순조회".Equals(e.sRQName) ? "1" : "3", 장운영구분);

            }

            log.Info("당일거래량상위응답처리[" + e.sRQName + "] end");
            if ("2".Equals(e.sPrevNext))
                당일거래량상위요청("당일거래량순조회".Equals(e.sRQName) ? "1":"3", 장운영구분, e.sPrevNext);
            else
            {
                //거래량 거래대금순 모두 조회했다고 가정
                if ( "2".Equals(장운영구분) && "당일거래대금순조회".Equals(e.sRQName))
                {
                    log.Info("거래대상설정완료 : " + 장운영구분);
                    dacStock.당일거래대상설정(inqDate, DateTime.Now.AddDays(1).ToString("yyyyMMdd"), 장운영구분);

                    System.Windows.Forms.Application.Exit();
                }
            }
        }

        public void 전일거래량상위요청(string flag, string sPrevNext = "0")
        {
            OpenAPI.SetInputValue("시장구분", "001"); // 000 : 전체, 001: 코스피, 101 : 코스닥

            // 조회구분 = 1:전일거래량 상위100종목, 2:전일거래대금 상위100종목
            OpenAPI.SetInputValue("조회구분", flag);

            OpenAPI.SetInputValue("순위시작", "");
            OpenAPI.SetInputValue("순위끝", "");

            OpenAPI.CommRqData(flag.Equals("1") ? "전일거래량순조회" : "전일거래대금순조회", "opt10031", !"2".Equals(sPrevNext) ? 0 : 2, "5001");
        }

        public void 전일거래량상위응답처리(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            int rowCount = OpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (int i = 0; i < rowCount; i++)
            {
                StockMyOrder myOrder = new StockMyOrder();

                string 종목코드 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                string 종목명 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                string 현재가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                string 전일대비기호 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일대비기호").Trim();
                string 전일대비 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일대비").Trim();
                string 거래량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "거래량").Trim();

                dacStock.전일_거래량거래대금순조회(inqDate, 종목코드, 종목명, 현재가, 전일대비기호, 전일대비, 거래량, "전일거래량순조회".Equals(e.sRQName) ? "1" : "2");
            }

            if ("2".Equals(e.sPrevNext))
                전일거래량상위요청("전일거래량순조회".Equals(e.sRQName) ? "1" : "2", e.sPrevNext);
        }

        public void 종목현재가조회응답처리(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            log.Info("종목현재가조회응답처리 start");

            int rowCount = OpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (int i = 0; i < rowCount; i++)
            {
                종목실시간정보 item = new 종목실시간정보();

                item.종목코드 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                item.종목명 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                item.현재가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                item.현재가 = item.현재가.Replace("-", "");

                item.기준가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "기준가").Trim();
                item.전일대비 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일대비").Trim();
                item.전일대비기호 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일대비기호").Trim();
                item.등락율 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "등락율").Trim();
                item.거래량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "거래량").Trim();
                item.거래대금 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "거래대금").Trim();
                item.체결량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "체결량").Trim();
                item.체결강도 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "체결강도").Trim();
                item.전일거래량대비 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일거래량대비").Trim();
                item.매도호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매도호가").Trim();
                item.매수호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매수호가").Trim();
                item.매도1차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매도1차호가").Trim();
                item.매도2차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매도2차호가").Trim();
                item.매도3차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매도3차호가").Trim();
                item.매도4차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매도4차호가").Trim();
                item.매도5차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매도5차호가").Trim();
                item.매수1차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매수1차호가").Trim();
                item.매수2차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매수2차호가").Trim();
                item.매수3차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매수3차호가").Trim();
                item.매수4차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매수4차호가").Trim();
                item.매수5차호가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매수5차호가").Trim();
                item.상한가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "상한가").Trim();
                item.하한가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "하한가").Trim();
                item.시가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "시가").Trim();
                item.고가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "고가").Trim();
                item.저가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "저가").Trim();
                item.종가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종가").Trim();
                item.체결시간 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "체결시간").Trim();
                item.예상체결가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "예상체결가").Trim();
                item.예상체결량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "예상체결량").Trim();
                item.자본금 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "자본금").Trim();
                item.액면가 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "액면가").Trim();
                item.시가총액 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "시가총액").Trim();
                item.주식수 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "주식수").Trim();
                item.호가시간 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "호가시간").Trim();
                item.일자 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "일자").Trim();
                item.우선매도잔량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "우선매도잔량").Trim();
                item.우선매수잔량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "우선매수잔량").Trim();
                item.우선매도건수 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "우선매도건수").Trim();
                item.우선매수건수 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "우선매수건수").Trim();
                item.총매도잔량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "총매도잔량").Trim();
                item.총매수잔량 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "총매수잔량").Trim();
                item.총매도건수 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "총매도건수").Trim();
                item.총매수건수 = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "총매수건수").Trim();

                dacStock.현재가갱신(inqDate, item.종목코드, item.현재가);

                dacStock.종목가격변동내역추가(inqDate, item);

                Thread.Sleep(10);
            }

            log.Info("종목현재가조회응답처리 end");
        }
    }
}
