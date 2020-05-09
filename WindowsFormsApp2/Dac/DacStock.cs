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

        #endregion

    }
}
