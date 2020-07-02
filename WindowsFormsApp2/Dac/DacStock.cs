using Dapper;
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
        public StockCode 종목정보조회(StockCode stockInfo )
        {
            //string query = Properties.Resources.getStockCode;
            //query = string.Format(query, stockInfo.stockCode, stockInfo.stockName);
            //return QuerySingle<StockCode>(query);

            DynamicParameters p = new DynamicParameters();
            p.Add("@sCode", stockInfo.stockCode);
            p.Add("@sName", stockInfo.stockName);

            StockCode result = QuerySingle<StockCode>("SP_종목정보조회", p);
            return result;
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

            //string query = Properties.Resources.insertStockDaily;
            //query = string.Format(query, stockInfo.inqDate,stockInfo.stockCode, stockInfo.stockName, stockInfo.currentPrice, stockInfo.traffic
            //    , stockInfo.이전거래량
            //    , stockInfo.diffBefore, stockInfo.upDownRate);

            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", stockInfo.inqDate);
            p.Add("@종목코드", stockInfo.stockCode);
            p.Add("@종목명", stockInfo.stockName);
            p.Add("@현재가", stockInfo.currentPrice);
            p.Add("@현재거래량", stockInfo.traffic);
            p.Add("@이전거래량", stockInfo.이전거래량);
            p.Add("@전일대비", stockInfo.diffBefore);
            p.Add("@등락율", stockInfo.upDownRate);
            p.Add("@매도호가_1", stockInfo.매도호가);
            p.Add("@매수호가_1", stockInfo.매수호가);

            return Execute("SP_당일거래량순종목추가", p);
        }

        public int 현재가갱신(string inqDate, StockDaily stockInfo)
        {
            //string query = Properties.Resources.종목현재가갱신;
            //query = string.Format(query, inqDate, stockInfo.stockCode, stockInfo.currentPrice );
            //return Execute(query);

            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);
            p.Add("@종목코드", stockInfo.stockCode);
            p.Add("@현재가", stockInfo.currentPrice);

            return Execute("SP_종목현재가갱신_보유종목평가금액갱신", p);
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

        public List<StockTarget> 매도요청중인종목전체조회(string inqDate)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);
            return Query<StockTarget>("SP_매도요청중인종목전체조회", p);
        }

        public int 주식상태매수요청중으로변경(string inqDate, StockTarget target)
        {
            string query = Properties.Resources.주식상태매수요청중으로변경;
            query = string.Format(query, inqDate, target.stockCode);
            return Execute(query);
        }

        public int 주식상태매도요청중으로변경(string inqDate, StockOrder order, int 매수수량, int 매수금액 )
        {
            //string query = Properties.Resources.주식상태매도요청중으로변경;
            //query = string.Format(query, inqDate, order.stockCode, 매수한수량, 매수한금액);

            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);
            p.Add("@종목코드", order.stockCode);
            p.Add("@매수수량_1", 매수수량);
            p.Add("@매입단가_1", 매수금액);

            return Execute("SP_매수완료처리_매도요청중으로변경", p);
        }

        public int 주식상태대기로변경(string inqDate, string stockCode, string Qty, string Price)
        {
            //string query = Properties.Resources.주식상태대기로변경;
            //query = string.Format(query, inqDate, order.stockCode);
            //return Execute(query);

            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);
            p.Add("@종목코드", stockCode);
            p.Add("@매도수량_1", Qty);
            p.Add("@매도단가_1", Price);

            return Execute("SP_주식상태대기로변경_금액계산", p);
        }

        public decimal 매도요청중인금액조회(string inqDate)
        {
            decimal result = 0;

            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);

            using (var connection = connectionFactory())
            {
                result = Dapper.SqlMapper.ExecuteScalar<decimal>(connection, "SP_매도요청중인금액조회", p, null, null, System.Data.CommandType.StoredProcedure );
                return result;
            }
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

        public List<StockMyOrder> 매도정정대상조회(string inqDate)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);

            return Query<StockMyOrder>("SP_매도정정대상조회", p);
        }

        public List<StockMyOrder> 매수완료된내역조회_myorderlist(string inqDate)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);

            return Query<StockMyOrder>("SP_매수완료된내역조회_myorderlist", p);
        }

        #endregion

        #region tbl_stock_order
        public int 주식주문이력추가(StockOrder stockOrder)
        {
            //string query = Properties.Resources.주식주문이력추가;
            //query = string.Format(query, stockOrder.inqDate, stockOrder.stockCode, stockOrder.stockName, stockOrder.Qty, stockOrder.Price
            //    , stockOrder.OrderType, stockOrder.Status, stockOrder.APIResult );

            //Execute(query);

            DynamicParameters p = new DynamicParameters();
            p.Add("@주문일자", stockOrder.inqDate);
            p.Add("@종목코드", stockOrder.stockCode);
            p.Add("@종목명", stockOrder.stockName);
            p.Add("@주문수량", stockOrder.Qty);
            p.Add("@주문단가", stockOrder.Price);
            p.Add("@주문타입", stockOrder.OrderType);
            p.Add("@주문상태", stockOrder.Status);
            p.Add("@APIResult", stockOrder.APIResult);
            p.Add("@원주문번호", stockOrder.orgOrderNo);

            Execute("SP_주문이력추가", p);

            return LastInsertID();
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
            //string query = Properties.Resources.매도완료업데이트대상조회;
            //query = string.Format(query, inqDate);
            //return Query<StockOrder>(query);

            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);

            return Query<StockOrder>("SP_매도완료업데이트대상조회", p);
        }

        public int 체결요청내역으로매도완료업데이트(StockOrder stockOrder)
        {
            string query = Properties.Resources.체결요청내역으로매도완료업데이트;
            query = string.Format(query, stockOrder.inqDate, stockOrder.stockCode, stockOrder.Qty
                , stockOrder.orderNo, stockOrder.Price);
            return Execute(query);
        }

        public StockOrder 매도요청중인주문한종목조회(string inqDate, StockTarget stockTarget)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);
            p.Add("@종목코드", stockTarget.stockCode);

            return QuerySingle<StockOrder>("SP_매도요청중인주문한종목조회", p);
        }

        public int 주문정보업데이트( string OrderSeq, string APIResult, string 주문번호 )
        {
            //string query = Properties.Resources.종목현재가갱신;
            //query = string.Format(query, inqDate, stockInfo.stockCode, stockInfo.currentPrice );
            //return Execute(query);

            DynamicParameters p = new DynamicParameters();
            p.Add("@orderSeq", OrderSeq);
            p.Add("@API결과", APIResult);
            p.Add("@주문번호", 주문번호 );

            return Execute("SP_주문정보업데이트", p);
        }

        public int 매도정정내역으로주문업데이트(StockMyOrder stockOrder)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@주문일자_v", stockOrder.orderDate);
            p.Add("@주문번호_v", stockOrder.orderNo);
            p.Add("@종목코드_v", stockOrder.stockCode);
            p.Add("@체결수량_v", stockOrder.confirmQty);
            p.Add("@체결가_v", stockOrder.confirmPrice);
            p.Add("@원주문번호_v", stockOrder.orgOrderNo);

            return Execute("SP_매도정정주문업데이트", p);
        }

        public int 주문번호업데이트(string inqDate, string stockCode, string OrderType, string OrderNo, string Qty, string Price )
        {
            //string query = Properties.Resources.종목현재가갱신;
            //query = string.Format(query, inqDate, stockInfo.stockCode, stockInfo.currentPrice );
            //return Execute(query);

            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자", inqDate);
            p.Add("@종목코드", stockCode);
            p.Add("@주문타입", OrderType);
            p.Add("@주문번호", OrderNo);
            p.Add("@주문수량", Qty);
            p.Add("@주문가격", Price);

            return Execute("SP_주문번호업데이트", p);
        }

        #endregion

        #region tbl_stock_price_history

        public int 종목가격변동내역추가(string 조회일자, string 종목코드, string 종목명, string 현재가 )
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@조회일자_1", 조회일자);
            p.Add("@종목코드_1", 종목코드);
            p.Add("@종목명_1", 종목명);
            p.Add("@현재가_1", 현재가);

            return Execute("SP_종목가격변동내역추가", p);
        }

        #endregion
    }
}
