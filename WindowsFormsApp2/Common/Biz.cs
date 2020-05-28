using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp2.Dac;
using WindowsFormsApp2.Data;

namespace WindowsFormsApp2.Common
{
    public class Biz
    {
        bool bDoneStockCodeUpdate = false;
        bool bTestMode = true;
        public AxKHOpenAPILib.AxKHOpenAPI OpenAPI { get; set; }

        public string AccountNo { get; set; }

        DacStock dacStock = new DacStock();
        string inqDate = DateTime.Now.ToString("yyyyMMdd");

        public List<StockCode> stockList { get; set; } = new List<StockCode>();

        // This method models an operation that may take a long time
        // to run. It can be cancelled, it can raise an exception,
        // or it can exit normally and return a result. These outcomes
        // are chosen randomly.
        public int TimeConsumingOperation(
            BackgroundWorker bw,
            int sleepPeriod)
        {
            int result = 0;

            Random rand = new Random();

            while (!bw.CancellationPending)
            {
                bool exit = false;

                // tbl_stock_code 를 초기화 및 업데이트를 한다.(하루에 한번)
                if (!bDoneStockCodeUpdate)
                    UpdateStockCode(bw);


                // 거래량 조회
                OpenAPI.SetInputValue("시장구분", "000"); // 000 : 전체, 001: 코스피, 101 : 코스닥
                OpenAPI.SetInputValue("주기구분", "5");
                OpenAPI.SetInputValue("거래량구분", "1000");
                OpenAPI.CommRqData("거래량순조회", "opt10024", 0, "5001");

                Thread.Sleep(5000);

                // 타겟 종목 선정
                dacStock.SetStockTarget(inqDate);

                
                Thread.Sleep(5000);

                // 매수/매도
                BuyAndSell();

                Thread.Sleep(5000);

                계좌주문별체결현황요청();

                Thread.Sleep(5000);

                매도완료처리및종목대기상태전환();
                if (exit)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// tbl_stock_code 를 초기화 및 업데이트를 한다.(하루에 한번)
        /// </summary>
        /// <param name="bw"></param>
        private void UpdateStockCode(BackgroundWorker bw)
        {
            for (int i = 0; i < stockList.Count; i++)
            {
                if (dacStock.GetStockCode(stockList[i]) == null)
                {
                    dacStock.insertStockCode(stockList[i]);
                }

                // (i + 1) / stockList.Count * 100
                bw.ReportProgress(i);

                if (bw.CancellationPending) break;
            }

            bDoneStockCodeUpdate = true;
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
                stockInfo.diffBefore = OpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "전일대비").Trim();

                if (dacStock.GetStockDailyInfo(inqDate, stockInfo.stockCode) == null)
                    dacStock.insertStockDaily(stockInfo);
                else
                    dacStock.현재가갱신(inqDate, stockInfo);
            }

            // 매수대상리스트에 현재가 갱신
            dacStock.거래량정보에서현재가갱신(inqDate);
        }

        public int 계좌주문별체결현황요청( string sPrevNext = "0" )
        {
            if (string.IsNullOrWhiteSpace(AccountNo)) return -1;

            // 주문일자 = YYYYMMDD (20170101 연도4자리, 월 2자리, 일 2자리 형식)
            OpenAPI.SetInputValue("주문일자", inqDate);

            // 계좌번호 = 전문 조회할 보유계좌번호
            OpenAPI.SetInputValue("계좌번호", AccountNo );

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
            OpenAPI.SetInputValue("시작주문번호", "");

            return OpenAPI.CommRqData("계좌별주문체결현황요청", "opw00009", !"2".Equals(sPrevNext)? 0:2, "1234");
        }

        public void 계좌별주문체결현황요청응답처리(string orderDate, object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e, string sPrevNext)
        {
            int rowCount = OpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);

            for ( int i = 0; i < rowCount; i++ )
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

                if ( !string.IsNullOrWhiteSpace(myOrder.orderNo))
                {
                    if (dacStock.체결내역있는지검사(myOrder) == null)
                        dacStock.체결내역한건등록(myOrder);
                    else
                        dacStock.체결내역업데이트(myOrder);
                }
                    
            }

            if ("2".Equals(sPrevNext)) 계좌주문별체결현황요청(sPrevNext);

        }

        
        public void BuyAndSell()
        {
            try
            {
                // 09시 10분부터 15시 20분까지 매수/매도
                if (DateTime.Now.ToString("HHmm").CompareTo("0930") < 0 ||
                    DateTime.Now.ToString("HHmm").CompareTo("1520") > 0) return;

                // 3시까지만 매수
                if (DateTime.Now.ToString("HHmm").CompareTo("1500") <= 0)
                {
                    List<StockTarget> targetList = dacStock.금일매수대상목록조회(inqDate);

                    for (int i = 0; i < targetList.Count; i++)
                    {
                        BuyStock(targetList[i]);
                        Thread.Sleep(500);
                    }
                }

                // 매수요청 후 체결된 애들 조회
                List<StockOrder> 매수완료로업데이트할대상리스트 = dacStock.매수완료업데이트대상및매도대상조회(inqDate);

                for ( int i = 0; i < 매수완료로업데이트할대상리스트.Count; i++ )
                {
                    dacStock.체결요청내역으로내주문업데이트(매수완료로업데이트할대상리스트[i]);

                    SellStock(매수완료로업데이트할대상리스트[i]);

                    Thread.Sleep(500);
                }

            } catch ( Exception ex )
            {

            }
        }

        static int TotalBalance = 10000000;

        // 한 종목당 30만원씩 매수
        static int EachStockBudget = 300000;

        public void BuyStock(StockTarget stock)
        {
            int price = price = int.Parse(stock.startPrice);

            // 시작가보다 현재가가 낮으면 현재가로 설정
            if ( price > int.Parse(stock.currentPrice))
                price = int.Parse(stock.currentPrice);

            int qty = EachStockBudget / price;

            if (qty <= 0) return;

            // 천만원만 우선 사용
            if (TotalBalance - price * qty<= 0) return;

            int resultCode = OpenAPI.SendOrder("종목신규매수", "8249", AccountNo, 1, stock.stockCode, qty, price , "00", "");

            TotalBalance -= price * qty;


            StockOrder order = new StockOrder();
            order.inqDate = stock.inqDate;
            order.stockCode = stock.stockCode;
            order.stockName = stock.stockName;
            order.Qty = qty.ToString();
            order.Price = price.ToString();
            order.OrderType = "매수";
            order.Status = "요청중";
            order.APIResult = resultCode.ToString();
            dacStock.주식주문이력추가(order);

            dacStock.주식상태매수요청중으로변경(stock.inqDate,stock);
        }

        public void SellStock(StockOrder stock)
        {
            int price = int.Parse(stock.Price);

            // 1% 이상 가격으로 매도
            price = (int)((double)price * 1.01);
            int qty = int.Parse(stock.Qty);

            int resultCode = OpenAPI.SendOrder("신규종목매도주문", "8289", AccountNo, 2, stock.stockCode, qty, price, "00", "");

            StockOrder order = new StockOrder();
            order.inqDate = stock.inqDate;
            order.stockCode = stock.stockCode;
            order.stockName = stock.stockName;
            order.Qty = qty.ToString();
            order.Price = price.ToString();
            order.OrderType = "매도";
            order.Status = "요청중";
            order.APIResult = resultCode.ToString();
            dacStock.주식주문이력추가(order);

            dacStock.주식상태매도요청중으로변경(stock.inqDate, stock);
        }

        public void 매도완료처리및종목대기상태전환()
        {
            List<StockOrder> 매도완료업데이트대상 = dacStock.매도완료업데이트대상조회(inqDate);

            for ( int i = 0; i < 매도완료업데이트대상.Count; i++ )
            {
                dacStock.체결요청내역으로매도완료업데이트(매도완료업데이트대상[i]);

                dacStock.주식상태대기로변경(inqDate, 매도완료업데이트대상[i]);

                int qty = string.IsNullOrWhiteSpace(매도완료업데이트대상[i].Qty) ? 0: int.Parse(매도완료업데이트대상[i].Qty);
                int price = string.IsNullOrWhiteSpace(매도완료업데이트대상[i].Price) ? 0 : int.Parse(매도완료업데이트대상[i].Price);
                
                if ( qty * price > 0)
                {
                    TotalBalance += qty * price;

                    // 세금제외
                    TotalBalance -= (int) (qty * price * 0.0033);
                }
            }
        }
    }
}
