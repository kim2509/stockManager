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
                if (order.orderNo.Equals(주문번호))
                {
                    order.ConfirmQty = 체결수량;
                    order.ConfirmPrice = 체결가;
                    order.Status = "완료";

                    log.Info("체결완료처리 order : " + JsonConvert.SerializeObject(order));

                    if (!"매도정정".Equals(주문구분))
                        dacStock.체결요청내역으로내주문업데이트(order);

                    if (Util.GetInt(order.Qty) == Util.GetInt(order.ConfirmQty))
                    {
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
                }
            }

            log.Info("체결완료처리 end");
        }

        private void 매수완료처리(StockOrder order)
        {
            log.Info("매수완료처리 new : " + JsonConvert.SerializeObject(order));

            if ("추가매수".Equals(order.OrderOption))
            {
                log.Info("추가매수");

                // tbl_stock_target 업데이트
                dacStock.주식상태매수완료처리로변경(order.inqDate, order.stockCode, Util.GetInt(order.ConfirmQty), Util.GetInt(order.ConfirmPrice), "Y");

                StockTarget target = dacStock.당일대상조회(order.inqDate, order.stockCode)[0];

                log.Info("매수완료처리:" + JsonConvert.SerializeObject(target));

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
            else
            {
                // tbl_stock_target 으로 업데이트
                dacStock.주식상태매수완료처리로변경(order.inqDate, order.stockCode, int.Parse(order.ConfirmQty), int.Parse(order.ConfirmPrice), "");
            }
        }

        public void 매도완료처리(string inqDate, StockOrder order)
        {
            log.Info("매도완료처리 new : " + JsonConvert.SerializeObject(order));

            // tbl_stock_target 업데이트
            dacStock.주식상태대기로변경(inqDate, order.stockCode, order.ConfirmQty, order.ConfirmPrice);

            // 그 사이 추가매수요청이 있을 수 있기때문에 이 요청을 취소한다.
            매수요청건조회및취소(inqDate, order.stockCode);

            if (Util.GetInt(order.ConfirmQty) * Util.GetInt(order.ConfirmPrice) > 0)
            {
                Biz.TotalBalance += Util.GetInt(order.ConfirmQty) * Util.GetInt(order.ConfirmPrice);

                // 세금제외
                Biz.TotalBalance -= (int)(Util.GetInt(order.ConfirmQty) * Util.GetInt(order.ConfirmPrice) * 0.0028);
            }
        }

        public void 매수요청건조회및취소(string inqDate, string stockCode)
        {
            log.Info("매수요청건조회및취소 start");

            List<StockOrder> orderList = dacStock.tbl_stock_order_주문조회(inqDate, stockCode, "매수", "요청중");

            for (int i = 0; i < orderList.Count; i++)
            {
                log.Info(string.Format("매수요청취소 stockCode : {0}, orderSeq:{1}, orderNo:{2}", stockCode, orderList[i].Seq.ToString(), orderList[i].orderNo));

                OpenAPI.SendOrder("매수요청취소", orderList[i].Seq, Biz.AccountNo, 3, stockCode, Util.GetInt(orderList[i].Qty),
                    Util.GetInt(orderList[i].Price), "00", orderList[i].orderNo);
            }

            log.Info("매수요청건조회및취소 end");
        }

        public void 매도정정완료처리(string inqDate, StockOrder order)
        {
            // tbl_stock_order 업데이트
            //dacStock.매도정정내역으로주문업데이트(order);
        }

    }
}
