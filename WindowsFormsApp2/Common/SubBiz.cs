using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp2.Dac;
using WindowsFormsApp2.Data;

namespace WindowsFormsApp2.Common
{
    public class SubBiz
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        DacStock dacStock = null;

        public APIManager OpenAPI { get; set; }

        public SubBiz(APIManager api)
        {
            dacStock = new DacStock();
            OpenAPI = api;
        }
        public void 체결완료처리(string inqDate, string stockCode, string 주문구분, string 주문번호, string 체결수량, string 체결가)
        {
            log.Info("체결완료처리 start inqDate:" + inqDate + " stockCode:" + stockCode + " 주문구분:" + 주문구분 + 
                " 주문번호:" + 주문번호 + " 체결수량:" + 체결수량 + " 체결가:" + 체결가);

            if (Util.GetInt(체결수량) <= 0 || Util.GetInt(체결가) <= 0) return;

            List<StockOrder> listOrders = dacStock.tbl_stock_order_주문조회(inqDate, stockCode, 주문구분, "요청중");

            for (int i = 0; i < listOrders.Count; i++)
            {
                StockOrder order = listOrders[i];
                if (주문번호.Equals(order.orderNo))
                {
                    order.ConfirmQty = 체결수량;
                    order.ConfirmPrice = 체결가;
                    
                    log.Info("체결완료처리 order : " + JsonConvert.SerializeObject(order));

                    if (Util.GetInt(order.Qty) == Util.GetInt(order.ConfirmQty))
                    {
                        order.Status = "완료";
                        dacStock.주문정보업데이트_byOrderSeq(order);

                        if ("매수".Equals(주문구분))
                        {
                            매수완료처리(order);
                        }
                        else if ("매도".Equals(주문구분))
                        {
                            매도완료처리(inqDate, order);
                        }
                        else if ("매도정정".Equals(주문구분))
                        {
                            매도정정완료처리(inqDate, order);
                        }
                    }
                    else
                        dacStock.주문정보업데이트_byOrderSeq(order);
                }
            }

            log.Info("체결완료처리 end");
        }

        private void 매수완료처리(StockOrder order)
        {
            log.Info("매수완료처리 new : " + JsonConvert.SerializeObject(order));

            StockTarget target = dacStock.당일대상조회(order.inqDate, order.stockCode)[0];

            log.Info("매수완료처리 변경전:" + JsonConvert.SerializeObject(target));

            // tbl_stock_target 업데이트
            dacStock.주식상태매수완료처리로변경(order.inqDate, order.stockCode
                , Util.GetInt(order.ConfirmQty), Util.GetInt(order.ConfirmPrice), "추가매수".Equals(order.OrderOption) ? "Y" : "" );

            target = dacStock.당일대상조회(order.inqDate, order.stockCode)[0];

            log.Info("매수완료처리 변경후:" + JsonConvert.SerializeObject(target));

            if ("추가매수".Equals(order.OrderOption))
            {
                log.Info("추가매수");

                List<StockOrder> listOrders = dacStock.tbl_stock_order_주문조회(order.inqDate, order.stockCode, "매도", "요청중");

                if (listOrders != null && listOrders.Count > 0 )
                {
                    StockOrder 매도요청중주문 = listOrders[0];

                    int resultCode = OpenAPI.매도취소요청(Biz.AccountNo, 매도요청중주문.Seq, 매도요청중주문.orderNo, 매도요청중주문.stockCode, 매도요청중주문.stockName
                        , 매도요청중주문.Qty, 매도요청중주문.Price);

                    if (resultCode == 0)
                        dacStock.주문상태변경(매도요청중주문.Seq, "취소완료");
                    else
                        dacStock.주문상태변경(매도요청중주문.Seq, "취소중오류");
                }
            }

            log.Info("매수완료처리 new end ");
        }

        public void 매도완료처리(string inqDate, StockOrder order)
        {
            log.Info("매도완료처리 new start: " + JsonConvert.SerializeObject(order));

            // tbl_stock_target 업데이트
            dacStock.주식상태대기로변경(inqDate, order.stockCode, order.ConfirmQty, order.ConfirmPrice);

            // 그 사이 추가매수요청이 있을 수 있기때문에 이 요청을 취소한다.
            매수요청건조회및취소(inqDate, order.stockCode, string.Empty);

            if (Util.GetInt(order.ConfirmQty) * Util.GetInt(order.ConfirmPrice) > 0)
            {
                Biz.TotalBalance += Util.GetInt(order.ConfirmQty) * Util.GetInt(order.ConfirmPrice);

                // 세금제외
                Biz.TotalBalance -= (int)(Util.GetInt(order.ConfirmQty) * Util.GetInt(order.ConfirmPrice) * 0.0028);
            }

            log.Info("매도완료처리 new end");
        }

        public void 매수요청건조회및취소(string inqDate, string stockCode, string updateTargetStatus)
        {
            log.Info("매수요청건조회및취소 start");

            List<StockOrder> orderList = dacStock.tbl_stock_order_주문조회(inqDate, stockCode, "매수", "요청중");

            for (int i = 0; i < orderList.Count; i++)
            {
                log.Info(string.Format("매수요청취소 stockCode : {0}, orderSeq:{1}, orderNo:{2}", stockCode, orderList[i].Seq.ToString(), orderList[i].orderNo));

                OpenAPI.SendOrder("매수요청취소", orderList[i].Seq, Biz.AccountNo, 3, stockCode, Util.GetInt(orderList[i].Qty),
                    Util.GetInt(orderList[i].Price), "00", orderList[i].orderNo);

                orderList[i].Status = "취소";

                dacStock.주문정보업데이트_byOrderSeq(orderList[i]);

            }

            if (!string.IsNullOrWhiteSpace(updateTargetStatus))
            {
                // target 상태변경
                dacStock.대상종목상태변경(inqDate, stockCode, updateTargetStatus);
            }

            log.Info("매수요청건조회및취소 end");
        }

        public void 매도정정완료처리(string inqDate, StockOrder order)
        {
            log.Info("매도정정완료처리 new start");

            // tbl_stock_order 업데이트
            dacStock.매도정정내역으로주문업데이트(inqDate, order.orderNo, order.stockCode, order.ConfirmQty, order.ConfirmPrice, order.orgOrderNo);

            log.Info("주식상태대기로변경 stockCode: " + order.stockCode + " confirmQty:" + order.ConfirmQty + " confirmPrice:" + order.ConfirmPrice);

            // tbl_stock_target 업데이트
            dacStock.주식상태대기로변경(inqDate, order.stockCode, order.ConfirmQty, order.ConfirmPrice);

            int qty = Util.GetInt(order.ConfirmQty);
            int price = Util.GetInt(order.ConfirmPrice);

            if (qty * price > 0)
            {
                Biz.TotalBalance += qty * price;

                // 세금제외
                Biz.TotalBalance -= (int)(qty * price * 0.0028);
            }

            log.Info("매도정정완료처리 new end");
        }

    }
}
