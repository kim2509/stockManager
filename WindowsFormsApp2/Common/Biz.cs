using Newtonsoft.Json;
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        bool bDoneStockCodeUpdate = false;
        bool bTestMode = true;
        public APIManager OpenAPI { get; set; }

        public TRBiz trBiz = null;

        public Biz(APIManager api)
        {
            OpenAPI = api;
        }

        public static string AccountNo { get; set; }

        DacStock dacStock = new DacStock();
        string inqDate = DateTime.Now.ToString("yyyyMMdd");

        public List<StockCode> stockList { get; set; } = new List<StockCode>();


        static decimal TotalBalance = 10000000;
        
        // 한 종목당 30만원씩 매수
        static int EachStockBudget = 300000;

        bool b장전시간외조회여부 = false;

        // This method models an operation that may take a long time
        // to run. It can be cancelled, it can raise an exception,
        // or it can exit normally and return a result. These outcomes
        // are chosen randomly.
        public int TimeConsumingOperation(
            BackgroundWorker bw,
            int sleepPeriod)
        {
            int result = 0;

            log.Info("TimeConsumingOperation start");

            try
            {

                Random rand = new Random();

                TotalBalance -= dacStock.매도요청중인금액조회(inqDate);

                while (!bw.CancellationPending)
                {
                    log.Info("loop start");

                    bool exit = false;

                    
                    // tbl_stock_code 를 초기화 및 업데이트를 한다.(하루에 한번)
                    if (!bDoneStockCodeUpdate)
                        UpdateStockCode(bw);

                    log.Info("loop UpdateStockCode finished");

                    /*
                    if (DateTime.Now.ToString("HHmm").CompareTo("0857") > 0 &&
                        DateTime.Now.ToString("HHmm").CompareTo("0901") < 0&& !b장전시간외조회여부)
                    {
                        b장전시간외조회여부 = true;

                        trBiz.당일거래량상위요청("1", "2");

                        Thread.Sleep(2000);

                        trBiz.당일거래량상위요청("3", "2");

                        Thread.Sleep(2000);

                        log.Info("loop 장전시간외대상조회 finished");
                    }
                    */


                    종목현재가조회();

                    // 거래량 조회
                    //trBiz.거래량순조회();

                    //if (dacStock.당일대상조회(inqDate, "").Count < 7)
                    //{
                    //    // 타겟 종목 선정
                    //    dacStock.SetStockTarget(inqDate);
                    //}


                    // 매수/매도
                    BuyAndSell();

                    log.Info("loop BuyAndSell finished");

                    // 매도요청중인 주문 업데이트 하기 위해.
                    trBiz.실시간미체결요청();

                    log.Info("loop 실시간미체결요청 finished");

                    trBiz.계좌주문별체결현황요청();

                    log.Info("loop 계좌주문별체결현황요청 finished");

                    Thread.Sleep(3000);

                    매도완료처리및종목대기상태전환();

                    log.Info("loop 매도완료처리및종목대기상태전환 finished");

                    손절완료처리및종목제외전환();

                    log.Info("loop 손절완료처리및종목제외전환 finished");

                    if (exit)
                    {
                        log.Info("loop exit");
                        break;
                    }

                    log.Info("loop end");
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("TimeConsumingOperation end");

            return result;
        }

        public void 종목현재가조회()
        {
            string 요일 = DateTime.Now.ToString("ddd");

            if (요일.Equals("토") || 요일.Equals("일")) return;

            // 09시부터 15시 30분까지 실행
            if (DateTime.Now.ToString("HHmm").CompareTo("0830") < 0 ||
                DateTime.Now.ToString("HHmm").CompareTo("1530") > 0) return;


            List<StockTarget> list = dacStock.당일대상조회(inqDate, "");

            int pageSize = 100;

            if (list.Count <= pageSize)
            {
                string stockCodeAr = string.Join(";", list.Select(i => i.stockCode).ToArray());
                OpenAPI.CommKwRqData(stockCodeAr, 0, list.Count, 0, "종목현재가조회", "1111");
            }
            else
            {
                int page = list.Count / pageSize;
                int remainer = list.Count % pageSize;

                if (remainer > 0)
                    page += 1;

                List<StockTarget> tempList = null;

                for (int i = 0; i < page; i++)
                {

                    if (i < page - 1)
                    {
                        tempList = list.Skip(i * pageSize).Take(pageSize).ToList();
                    }
                    else
                    {
                        tempList = list.Skip(i * pageSize).Take(remainer).ToList();
                    }

                    string stockCodeAr = string.Join(";", tempList.Select(k => k.stockCode).ToArray());
                    OpenAPI.CommKwRqData(stockCodeAr, 0, tempList.Count, 0, "종목현재가조회", "1111");
                }
            }

            log.Info("loop CommKwRqData finished");

            Thread.Sleep(2000);
        }

        /// <summary>
        /// tbl_stock_code 를 초기화 및 업데이트를 한다.(하루에 한번)
        /// </summary>
        /// <param name="bw"></param>
        private void UpdateStockCode(BackgroundWorker bw)
        {
            for (int i = 0; i < stockList.Count; i++)
            {
                if (dacStock.종목정보조회(stockList[i]) == null)
                {
                    dacStock.insertStockCode(stockList[i]);
                }

                // (i + 1) / stockList.Count * 100
                bw.ReportProgress(i);

                if (bw.CancellationPending) break;
            }

            bDoneStockCodeUpdate = true;
        }

        public void BuyAndSell()
        {
            try
            {
                log.Info("BuyAndSell start");

                string 요일 = DateTime.Now.ToString("ddd");

                if (요일.Equals("토") || 요일.Equals("일")) return;

                // 09시 10분부터 15시 20분까지 매수/매도
                if (DateTime.Now.ToString("HHmm").CompareTo("0900") < 0 ||
                    DateTime.Now.ToString("HHmm").CompareTo("1530") > 0) return;
                
                // 3시까지만 매수(10종목만 매수)
                if (DateTime.Now.ToString("HHmm").CompareTo("1500") <= 0 ) //&& 매도요청중인종목리스트.Count < 10)
                {
                    List<StockTarget> targetList = dacStock.금일매수대상목록조회(inqDate);

                    for (int i = 0; i < targetList.Count; i++)
                    {
                        종목증감정보 정보 = dacStock.종목최근등락률조회(inqDate, targetList[i].stockCode);
                        종목실시간정보 실시간정보 = dacStock.최근한종목가격변동내역조회(inqDate, targetList[i].stockCode);

                        log.Info("종목최근등락률조회 결과:" + JsonConvert.SerializeObject(정보));
                        log.Info(string.Format("체결강도 : {0} 총매도잔량 : {1} 총매수잔량 : {2}", 실시간정보.체결강도, 실시간정보.총매도잔량, 실시간정보.총매수잔량));

                        if (정보 == null) continue;

                        log.Info("매수대상 :" + JsonConvert.SerializeObject(targetList[i]));

                        if ( 매수여부판단(정보, 실시간정보) )
                        {
                            int price = Util.GetInt(targetList[i].currentPrice);

                            int waterCnt = 0;
                            if (!string.IsNullOrWhiteSpace(targetList[i].waterCnt)) waterCnt = int.Parse(targetList[i].waterCnt);

                            BuyStock(inqDate, targetList[i].stockCode, targetList[i].stockName, price, "", waterCnt);
                        }
                    }
                }

                // 매수요청 후 체결된 애들 조회(tbl_stock_myorderlist)
                매수완료처리및매도요청();

                // tbl_stock_target 에서 조회
                List<StockTarget> 매도요청중인종목리스트 = dacStock.매도요청중인종목전체조회(inqDate);


                if (DateTime.Now.ToString("HHmm").CompareTo("1510") < 0)
                {
                    추가매수(매도요청중인종목리스트);
                }

                // -5% 밑으로 인 애들 조회해서 현재가로 손절한다.
                for (int i = 0; i < 매도요청중인종목리스트.Count; i++)
                {
                    string tmpRate = 매도요청중인종목리스트[i].손익률;
                    if (string.IsNullOrWhiteSpace(tmpRate)) tmpRate = "0";

                    if (float.Parse(tmpRate) <= -5)
                    {
                        StockOrder order = dacStock.매도요청중인주문한종목조회(inqDate, 매도요청중인종목리스트[i].stockCode);
                        if (order == null) continue;

                        int price = int.Parse(order.Price);
                        int qty = int.Parse(order.Qty);
                        // 0.5% 더 낮은 금액으로 손절요청
                        price -= (int)(price * 0.002);

                        매도정정요청(order.inqDate, order.orderNo, order.stockCode, order.stockName, qty, price);
                    }
                }

                // 3시 20분부터는 매도요청중인 종목을 손절한다.
                if (DateTime.Now.ToString("HHmm").CompareTo("1510") >= 0)
                {
                    for (int i = 0; i < 매도요청중인종목리스트.Count; i++)
                    {
                        StockOrder order = dacStock.매도요청중인주문한종목조회(inqDate, 매도요청중인종목리스트[i].stockCode);

                        if (order == null) continue;

                        int price = int.Parse(order.Price);
                        int qty = int.Parse(order.Qty);

                        매도정정요청(order.inqDate, order.orderNo, order.stockCode, order.stockName, qty, price);
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                log.Info("BuyAndSell end");
            }
        }

        public bool 매수여부판단(종목증감정보 정보, 종목실시간정보 실시간정보)
        {
            bool bBuy = false;

            /*
            // 급상승종목이라 판단해서 바로 올라탐.(총매도잔량이 총매수잔량보다 2배이면서 체결강도 100이 넘는경우)
            if (Util.GetDouble(실시간정보.총매도잔량) / Util.GetDouble(실시간정보.총매수잔량) >= 2.0 && Util.GetDouble(실시간정보.체결강도) > 100)
            {
                bBuy = true;
            }
            else 
            */
            if (정보.총매도잔량증가율 > 0 &&
                정보.총매수잔량증가율 < 0 &&
                정보.체결강도증가율 > 0 &&
                Util.GetDouble(실시간정보.총매도잔량) / Util.GetDouble(실시간정보.총매수잔량) >= 1.5 &&
                Util.GetDouble(실시간정보.체결강도) > 90 &&
                정보.갯수 == 5 )
            {
                bBuy = true;
            }
            else
                bBuy = false;

            return bBuy;
        }

        private void 추가매수(List<StockTarget> 매도요청중인종목리스트)
        {
            log.Info("추가매수 start");

            // -2.5% 밑으로 인 애들 조회해서 추가매수한다.
            for (int i = 0; i < 매도요청중인종목리스트.Count; i++)
            {
                log.Info("매도요청중인종목리스트[i]:" + 매도요청중인종목리스트[i].stockCode + " 손익률:" + 매도요청중인종목리스트[i].손익률);
                string tmpRate = 매도요청중인종목리스트[i].손익률;
                if (string.IsNullOrWhiteSpace(tmpRate)) tmpRate = "0";

                if (float.Parse(tmpRate) <= -2.5 && float.Parse(tmpRate) > -4.5)
                {
                    log.Info("매도요청중인종목[i]:" + JsonConvert.SerializeObject(매도요청중인종목리스트[i]));

                    StockOrder order = dacStock.매도요청중인주문한종목조회(inqDate, 매도요청중인종목리스트[i].stockCode);

                    if ( order == null )
                    {
                        log.Info("order 가 null 이여 씨벌~");
                        continue;
                    }
                    
                    // 현재가
                    int price = Util.GetInt(order.Price);
                    int waterCnt = Util.GetInt(매도요청중인종목리스트[i].waterCnt);

                    bool b추가매수가능여부 = true;

                    List<StockOrder> 주문리스트 = dacStock.tbl_stock_order_주문조회(inqDate, order.stockCode, "매수", "요청중");
                    if (주문리스트 != null && 주문리스트.Count > 0)
                    {
                        for (int j = 0; j < 주문리스트.Count; j++)
                        {
                            if ("추가매수".Equals(주문리스트[j].OrderOption))
                                b추가매수가능여부 = false;
                        }
                    }


                    if (b추가매수가능여부 && waterCnt <= 1)
                        BuyStock(inqDate, order.stockCode, order.stockName, price, "추가매수", waterCnt);
                }
            }

            log.Info("추가매수 end");
        }

        private void 매수완료처리및매도요청()
        {
            log.Info("매수완료처리및매도요청 start");

            List<StockMyOrder> 매수완료리스트 = dacStock.매수완료된내역조회_myorderlist(inqDate);

            for (int i = 0; i < 매수완료리스트.Count; i++)
            {
                StockMyOrder myOrder = 매수완료리스트[i];

                // 체결되지 않음.
                if (string.IsNullOrWhiteSpace(myOrder.confirmQty) || "0".Equals(myOrder.confirmQty)) continue;

                // 동일종목의 tbl_stock_order 리스트 조회
                List<StockOrder> orderList = dacStock.tbl_stock_order_주문조회(inqDate, 매수완료리스트[i].stockCode, "매수", "요청중");

                StockOrder order = null;

                for (int j = 0; j < orderList.Count; j++)
                {
                    
                    order = orderList[j];

                    log.Info("myOrder: " + JsonConvert.SerializeObject(myOrder));
                    log.Info("order: " + JsonConvert.SerializeObject(order));

                    if ( myOrder.orderNo.Equals( order.orderNo ) ||
                        (int.Parse(myOrder.Qty.Trim()) == int.Parse(order.Qty.Trim()) && int.Parse(myOrder.Price.Trim()) == int.Parse(order.Price.Trim())))
                    {
                        if (string.IsNullOrWhiteSpace(order.orderNo))
                        {
                            // 주문번호가 업데이트 안돼있을 경우 대비
                            order.orderNo = myOrder.orderNo;

                            log.Info("order.orderNo:" + order.orderNo + " seq:" + order.Seq + " stockCode:" + order.stockCode);

                            dacStock.주문정보업데이트(order.Seq, "", order.orderNo, order.stockCode);
                        }

                        // 체결가로 변경
                        //order.Qty = myOrder.confirmQty;
                        //order.Price = myOrder.confirmPrice;

                        // tbl_stock_order 의 매수요청중을 매수완료로 변경
                        dacStock.체결요청내역으로내주문업데이트(order);

                        // 1건만 찾음.
                        break;
                    }
                    else
                    {
                        // 다르면 다른 주문건이라서 매도요청하면 안됨.
                        order = null;
                        log.Info("주문건이 서로 다름.");
                    }
                }

                if (order != null)
                {
                    if ("추가매수".Equals(order.OrderOption))
                    {
                        log.Info("추가매수");

                        // tbl_stock_target 업데이트
                        dacStock.주식상태매도요청중으로변경(order.inqDate, order.stockCode, Util.GetInt(order.Qty), Util.GetInt(order.Price), "Y");

                        StockTarget target = dacStock.당일대상조회(order.inqDate, order.stockCode)[0];

                        log.Info("추가매수완료처리대상:" + JsonConvert.SerializeObject(target));

                        // 추가매수로 매입단가가 내려갔을 테니 매도 목표가도 다시 정정한다.
                        int price = int.Parse(target.매입단가);

                        StockOrder 매도요청중주문 = dacStock.tbl_stock_order_주문조회(order.inqDate, order.stockCode, "매도", "요청중")[0];

                        int resultCode = OpenAPI.매도취소요청(AccountNo, 매도요청중주문.Seq, 매도요청중주문.orderNo, 매도요청중주문.stockCode, 매도요청중주문.stockName
                            , 매도요청중주문.Qty, 매도요청중주문.Price);

                        //매도정정요청(order.inqDate, 매도요청중주문.orderNo, order.stockCode, order.stockName, int.Parse(order.Qty), int.Parse(order.Price));

                        if (resultCode == 0 )
                            dacStock.주문상태변경(매도요청중주문.Seq, "취소완료");
                        else
                            dacStock.주문상태변경(매도요청중주문.Seq, "취소중오류");

                        StockOrder 매도요청할주문 = new StockOrder();
                        매도요청할주문.inqDate = inqDate;
                        매도요청할주문.stockCode = target.stockCode;
                        매도요청할주문.stockName = target.stockName;
                        매도요청할주문.Qty = target.보유수;
                        매도요청할주문.Price = price.ToString();

                        SellStock(매도요청할주문);
                    }
                    else
                    {
                        log.Info("1%매도");

                        dacStock.주식상태매도요청중으로변경(order.inqDate, order.stockCode, int.Parse(order.Qty), int.Parse(order.Price), "");

                        // 1% 올려서 매도요청
                        SellStock(order);
                    }
                }
            }

            log.Info("매수완료처리및매도요청 end");
        }

        public void BuyStock(string inqDate, string stockCode, string stockName, int price, string option, int waterCnt)
        {
            int qty = EachStockBudget / price;

            if (qty <= 0) return;

            // 천만원만 우선 사용 (추가매수일땐 무한정)
            if (string.IsNullOrWhiteSpace(option) && TotalBalance - price * qty <= 0)
            {
                log.Info("돈없어서 못사는중 : " + TotalBalance + " 종목코드:" + stockCode + " 종목명:" + stockName );
                return;
            }

            if ("추가매수".Equals(option))
            {
                if (waterCnt == 0) qty = qty * 2;
                else if (waterCnt == 1) qty = qty * 4;
            }

            StockOrder order = new StockOrder();
            order.inqDate = inqDate;
            order.stockCode = stockCode;
            order.stockName = stockName;
            order.Qty = qty.ToString();
            order.Price = price.ToString();
            order.OrderType = "매수";
            order.Status = "요청중";
            order.OrderOption = option;
            int orderSeq = dacStock.주식주문이력추가(order);

            int resultCode = OpenAPI.SendOrder("종목신규매수", orderSeq.ToString(), AccountNo, 1, order.stockCode = stockCode, qty, price, "00", "");

            order.APIResult = resultCode.ToString();

            dacStock.주문정보업데이트(orderSeq.ToString(), order.APIResult, "");

            Console.WriteLine("매수주문[" + orderSeq.ToString() + "] : " + Newtonsoft.Json.JsonConvert.SerializeObject(order));

            // 추가매수일땐 무한정
            if (string.IsNullOrWhiteSpace(option))
                TotalBalance -= price * qty;

            dacStock.주식상태매수요청중으로변경(inqDate, stockCode);
        }

        public void SellStock(StockOrder stock)
        {
            log.Info("SellStock start : " + JsonConvert.SerializeObject(stock));

            List<StockOrder> orderlist = dacStock.tbl_stock_order_주문조회(inqDate, stock.stockCode, "매도", "요청중");
            if (orderlist.Count > 0)
            {
                log.Error("매도요청중인 주문이 존재!!!");
                return;
            }

            int price = int.Parse(stock.Price);

            int 매수금액 = price;

            // 1% 이상 가격으로 매도
            price = (int)((double)price * 1.01);
            int qty = int.Parse(stock.Qty);

            int 매수수량 = qty;

            StockOrder order = new StockOrder();
            order.inqDate = stock.inqDate;
            order.stockCode = stock.stockCode;
            order.stockName = stock.stockName;
            order.Qty = qty.ToString();
            order.Price = price.ToString();
            order.OrderType = "매도";
            order.OrderOption = "";
            order.Status = "요청중";
            int orderSeq = dacStock.주식주문이력추가(order);

            int resultCode = OpenAPI.SendOrder("신규종목매도주문", orderSeq.ToString(), AccountNo, 2, stock.stockCode, qty, price, "00", "");

            order.APIResult = resultCode.ToString();

            dacStock.주문정보업데이트(orderSeq.ToString(), order.APIResult, "");

            log.Info("SellStock end");
        }

        /// <summary>
        /// 현재가로 손절
        /// </summary>
        /// <param name="stock"></param>
        public void 매도정정요청(string inqDate, string orgOrderNo, string stockCode, string stockName, int qty, int price)
        {
            log.Info("매도정정요청 start: " + inqDate + " orderNo:" + orgOrderNo + " stockCode:" + stockCode + " stockName:" + stockName
                + " price:" + price.ToString() + " qty:" + qty.ToString());

            if (string.IsNullOrWhiteSpace(orgOrderNo))
            {
                log.Error("매도정정요청 주문번호 없음!!!");
                return;
            }

            List<StockOrder> orderlist = dacStock.tbl_stock_order_주문조회(inqDate, stockCode, "매도정정", "요청중");
            if (orderlist.Count > 0)
            {
                log.Error("매도정정요청중인 주문이 존재!!!");
                return;
            }

            StockOrder order = new StockOrder();
            order.inqDate = inqDate;
            order.stockCode = stockCode;
            order.stockName = stockName;
            order.Qty = qty.ToString();
            order.Price = price.ToString();
            order.OrderType = "매도정정";
            order.OrderOption = "";
            order.Status = "요청중";
            order.orgOrderNo = orgOrderNo;

            int orderSeq = dacStock.주식주문이력추가(order);

            log.Info("매도정정주문 orderSeq:" + orderSeq);

            // 매도정정요청
            int resultCode = OpenAPI.SendOrder("매도정정요청", orderSeq.ToString(), AccountNo, 6, order.stockCode, qty, price, "00", order.orgOrderNo);

            order.APIResult = resultCode.ToString();

            dacStock.주문정보업데이트(orderSeq.ToString(), order.APIResult, "");

            log.Info("매도정정 주문 end[" + orderSeq.ToString() + "] : " + Newtonsoft.Json.JsonConvert.SerializeObject(order));
        }

        public void 매도완료처리및종목대기상태전환()
        {
            log.Info("매도완료처리및종목대기상태전환 start");

            // tbl_stock_myorderlist 에서 불러옴
            List<StockMyOrder> 매도완료업데이트대상 = dacStock.매도완료업데이트대상조회(inqDate);

            log.Info("매도완료업데이트대상 count:" + 매도완료업데이트대상.Count);

            for (int i = 0; i < 매도완료업데이트대상.Count; i++)
            {
                List<StockOrder> orderList = dacStock.tbl_stock_order_주문조회(inqDate, 매도완료업데이트대상[i].stockCode, "매도", "요청중");

                if (orderList == null || orderList.Count < 1) continue;

                StockOrder 매도요청중인주문 = orderList[0];

                if (매도요청중인주문 == null) continue;

                log.Info("매도완료업데이트대상: " + JsonConvert.SerializeObject(매도완료업데이트대상[i]));
                log.Info("매도요청중인주문: " + JsonConvert.SerializeObject(매도요청중인주문));

                int qty = Util.GetInt(매도완료업데이트대상[i].Qty);
                int price = Util.GetInt(매도완료업데이트대상[i].Price);

                int orderQty = Util.GetInt(매도요청중인주문.Qty);
                int orderPrice = Util.GetInt(매도요청중인주문.Price);

                // 주문번호가 다르면 제외
                if (!string.IsNullOrWhiteSpace(매도요청중인주문.orderNo) && !매도요청중인주문.orderNo.Equals(매도완료업데이트대상[i].orderNo))
                {
                    log.Info("주문번호 다름");
                    continue;
                }

                // 수량이나 가격이 다를 경우 제외
                if (qty != orderQty || price != orderPrice)
                {
                    log.Info("수량 or 금액 다름 ");
                    continue;
                }

                log.Info("매도완료업데이트 진행");

                // tbl_stock_order 업데이트
                dacStock.주문정보업데이트(매도요청중인주문.Seq, 매도완료업데이트대상[i].orderNo, 매도완료업데이트대상[i].Qty, 매도완료업데이트대상[i].Price
                    , "매도", "완료", 매도요청중인주문.APIResult);

                // tbl_stock_target 업데이트
                dacStock.주식상태대기로변경(inqDate, 매도완료업데이트대상[i].stockCode, 매도완료업데이트대상[i].Qty, 매도완료업데이트대상[i].Price);

                if (qty * price > 0)
                {
                    TotalBalance += qty * price;

                    // 세금제외
                    TotalBalance -= (int)(qty * price * 0.0033);
                }
            }

            log.Info("매도완료처리및종목대기상태전환 end");
        }

        public void 손절완료처리및종목제외전환()
        {
            log.Info("손절완료처리및종목제외전환 start");
            // tbl_stock_myorderlist 에서 불러옴
            List<StockMyOrder> 매도정정대상 = dacStock.매도정정대상조회(inqDate);

            for (int i = 0; i < 매도정정대상.Count; i++)
            {
                log.Info("손절완료처리및종목제외전환 : " + JsonConvert.SerializeObject(매도정정대상[i]));

                if (string.IsNullOrWhiteSpace(매도정정대상[i].confirmQty) || "0".Equals(매도정정대상[i].confirmQty))
                {
                    log.Info("confirmQty is 0");
                    continue;
                }

                if (매도정정대상[i].confirmPrice == null) 매도정정대상[i].confirmPrice = "0";

                if (string.IsNullOrWhiteSpace(매도정정대상[i].confirmPrice) || "0".Equals(매도정정대상[i].confirmPrice))
                {
                    log.Info("confirmPrice is 0");
                    continue;
                }

                List<StockOrder> orderList = dacStock.tbl_stock_order_주문조회(inqDate, 매도정정대상[i].stockCode, "매도정정", "요청중");

                if (orderList == null || orderList.Count < 1)
                {
                    log.Info("해당 order 없음");
                    continue;
                }

                StockOrder order = orderList[0];

                if (매도정정대상[i].Qty == null) 매도정정대상[i].Qty = "0";
                if (string.IsNullOrWhiteSpace(order.Qty)) order.Qty = "0";

                int myOrderQty = int.Parse(매도정정대상[i].Qty);
                int orderQty = int.Parse(order.Qty);

                if (myOrderQty != orderQty)
                {
                    log.Info("Qty 다름");
                    continue;
                }

                if (매도정정대상[i].Price == null) 매도정정대상[i].Price = "0";
                if (string.IsNullOrWhiteSpace(order.Price)) order.Price = "0";

                int myOrderPrice = int.Parse(매도정정대상[i].Price);
                int orderPrice = int.Parse(order.Price);

                if (myOrderPrice != orderPrice)
                {
                    log.Info("Price 다름");
                    continue;
                }

                // tbl_stock_order 업데이트
                dacStock.매도정정내역으로주문업데이트(매도정정대상[i]);

                log.Info("주식상태대기로변경 stockCode: " + 매도정정대상[i].stockCode + " confirmQty:" + 매도정정대상[i].confirmQty + " confirmPrice:" + 매도정정대상[i].confirmPrice);
                // tbl_stock_target 업데이트
                dacStock.주식상태대기로변경(inqDate, 매도정정대상[i].stockCode, 매도정정대상[i].confirmQty, 매도정정대상[i].confirmPrice);

                int qty = string.IsNullOrWhiteSpace(매도정정대상[i].confirmQty) ? 0 : int.Parse(매도정정대상[i].confirmQty);
                int price = string.IsNullOrWhiteSpace(매도정정대상[i].confirmPrice) ? 0 : int.Parse(매도정정대상[i].confirmPrice);

                if (qty * price > 0)
                {
                    TotalBalance += qty * price;

                    // 세금제외
                    TotalBalance -= (int)(qty * price * 0.0028);
                }
            }

            log.Info("손절완료처리및종목제외전환 end");
        }
    }
}
