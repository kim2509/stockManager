using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp2.Dac;
using WindowsFormsApp2.Data;

namespace WindowsFormsApp2.Common
{
    public class BuyBiz
    {
        DacStock dacStock = null;

        public APIManager OpenAPI { get; set; }

        public BuyBiz(APIManager api)
        {
            dacStock = new DacStock();
            OpenAPI = api;
        }
        public void 체결완료처리(string inqDate, string stockCode, string 주문구분, string 체결수량, string 체결가)
        {
            if (Util.GetInt(체결수량) <= 0 || Util.GetInt(체결가) <= 0) return;

            List<StockOrder> listOrders = dacStock.tbl_stock_order_주문조회(inqDate, stockCode, 주문구분, "요청중");

            for (int i = 0; i < listOrders.Count; i++)
            {
                StockOrder order = listOrders[i];
                if (order.orderNo.Equals(OpenAPI.GetChejanData(9203).Trim()))
                {
                    if (Util.GetInt(order.Qty) == Util.GetInt(체결수량) && Util.GetInt(order.Price) == Util.GetInt(체결가))
                    {

                    }
                }
            }
        }
    }
}
