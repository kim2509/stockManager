using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp2.Common;
using WindowsFormsApp2.Data;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsFormsApp2.Dac
{
    public class DacStock : DBHelper
    {
        #region tbl_stock_code
        public StockCode GetStockCode(StockCode stockInfo )
        {
            string query = Properties.Resources.getStockCode;
            query = string.Format(query, stockInfo.stockCode, stockInfo.stockName);
            return QuerySingle<StockCode>(query);
        }

        public int insertStockCode(StockCode stockInfo)
        {
            if (string.IsNullOrWhiteSpace(stockInfo.stockCode) || string.IsNullOrWhiteSpace(stockInfo.stockName)) return -1;

            string query = Properties.Resources.insertStockCode;
            query = string.Format(query, stockInfo.stockCode, stockInfo.stockName);
            return Execute(query);
        }

        #endregion

        #region tbl_stock_daily

        public StockCode GetStockDailyInfo(string inqDate, string stockCode )
        {
            string query = Properties.Resources.getStockDailyInfo;
            query = string.Format(query, inqDate, stockCode);
            return QuerySingle<StockCode>(query);
        }

        public int insertStockDaily(StockDaily stockInfo)
        {
            if (string.IsNullOrWhiteSpace(stockInfo.stockCode) || string.IsNullOrWhiteSpace(stockInfo.stockName)) return -1;

            string query = Properties.Resources.insertStockDaily;
            query = string.Format(query, stockInfo.inqDate,stockInfo.stockCode, stockInfo.stockName, stockInfo.currentPrice, stockInfo.traffic, stockInfo.diffBefore, stockInfo.upDownRate);
            return Execute(query);
        }

        public int 현재가갱신(string inqDate, StockDaily stockInfo)
        {
            string query = Properties.Resources.종목현재가갱신;
            query = string.Format(query, inqDate, stockInfo.stockCode, stockInfo.currentPrice );
            return Execute(query);
        }

        #endregion

        #region tbl_stock_target

        /// <summary>
        /// 오늘의 대상 주식리스트를 생성한다.
        /// </summary>
        /// <param name="inqDate"></param>
        /// <returns></returns>
        public int SetStockTarget(string inqDate)
        {
            string query = Properties.Resources.setStockTarget;
            query = string.Format(query, inqDate);
            return Execute(query);
        }

        public int 거래량정보에서현재가갱신(string inqDate)
        {
            string query = Properties.Resources.거래량정보에서현재가갱신;
            query = string.Format(query, inqDate);
            return Execute(query);
        }

        public List<StockTarget> 금일매수대상목록조회(string inqDate)
        {
            string query = Properties.Resources.금일매수대상종목조회;
            query = string.Format(query, inqDate);
            return Query<StockTarget>(query);
        }

        public int 주식상태매수요청중으로변경(string inqDate, StockTarget target)
        {
            string query = Properties.Resources.주식상태매수요청중으로변경;
            query = string.Format(query, inqDate, target.stockCode);
            return Execute(query);
        }

        public int 주식상태매도요청중으로변경(string inqDate, StockOrder order)
        {
            string query = Properties.Resources.주식상태매도요청중으로변경;
            query = string.Format(query, inqDate, order.stockCode);
            return Execute(query);
        }

        public int 주식상태대기로변경(string inqDate, StockOrder order)
        {
            string query = Properties.Resources.주식상태대기로변경;
            query = string.Format(query, inqDate, order.stockCode);
            return Execute(query);
        }

        #endregion

        #region tbl_stock_myorderlist

        public int 체결내역한건등록(StockMyOrder myOrder)
        {
            string query = Properties.Resources.체결내역한건등록;
            query = string.Format(query, myOrder.stockBondGubun, myOrder.orderNo, myOrder.stockCode, myOrder.sellFlag, myOrder.orderType, myOrder.Qty
                , myOrder.Price, myOrder.checkQty, myOrder.reserveDeny, myOrder.confirmNo, myOrder.acceptType
                , myOrder.orgOrderNo, myOrder.stockName, myOrder.payType, myOrder.creditTransType, myOrder.confirmQty
                , myOrder.confirmPrice, myOrder.commType, myOrder.modifyFlag, myOrder.confirmedTime, myOrder.orderDate );

            return Execute(query);
        }

        public StockMyOrder 체결내역있는지검사(StockMyOrder myOrder)
        {
            string query = Properties.Resources.체결내역있는지검사;
            query = string.Format(query, myOrder.orderDate, myOrder.orderNo, myOrder.stockCode);
            return QuerySingle<StockMyOrder>(query);
        }

        public int 체결내역업데이트(StockMyOrder myOrder)
        {
            string query = Properties.Resources.체결내역업데이트;
            query = string.Format(query, myOrder.orderDate, myOrder.orderNo, myOrder.stockCode, myOrder.stockBondGubun, myOrder.sellFlag
                , myOrder.orderType, myOrder.Qty, myOrder.Price, myOrder.reserveDeny, myOrder.confirmNo, myOrder.acceptType
                , myOrder.orgOrderNo, myOrder.stockName, myOrder.payType, myOrder.creditTransType, myOrder.confirmQty
                , myOrder.confirmPrice, myOrder.commType, myOrder.modifyFlag, myOrder.confirmedTime);

            return Execute(query);
        }

        #endregion

        #region tbl_stock_order
        public int 주식주문이력추가(StockOrder stockOrder)
        {
            string query = Properties.Resources.주식주문이력추가;
            query = string.Format(query, stockOrder.inqDate, stockOrder.stockCode, stockOrder.stockName, stockOrder.Qty, stockOrder.Price
                , stockOrder.OrderType, stockOrder.Status, stockOrder.APIResult );
            return Execute(query);
        }

        public List<StockOrder> 매수완료업데이트대상및매도대상조회(string inqDate)
        {
            string query = Properties.Resources.매수완료업데이트대상및매도대상조회;
            query = string.Format(query, inqDate);
            return Query<StockOrder>(query);
        }

        public int 체결요청내역으로내주문업데이트(StockOrder stockOrder)
        {
            string query = Properties.Resources.체결요청내역으로내주문업데이트;
            query = string.Format(query, stockOrder.inqDate, stockOrder.stockCode, stockOrder.Qty
                , stockOrder.orderNo, stockOrder.Price);
            return Execute(query);
        }

        public List<StockOrder> 매도완료업데이트대상조회(string inqDate)
        {
            string query = Properties.Resources.매도완료업데이트대상조회;
            query = string.Format(query, inqDate);
            return Query<StockOrder>(query);
        }

        public int 체결요청내역으로매도완료업데이트(StockOrder stockOrder)
        {
            string query = Properties.Resources.체결요청내역으로매도완료업데이트;
            query = string.Format(query, stockOrder.inqDate, stockOrder.stockCode, stockOrder.Qty
                , stockOrder.orderNo, stockOrder.Price);
            return Execute(query);
        }

        #endregion
    }
}
