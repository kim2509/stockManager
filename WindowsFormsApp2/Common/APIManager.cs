using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp2.Common
{
    public class APIManager
    {
        private static Mutex mut = new Mutex();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AxKHOpenAPILib.AxKHOpenAPI OpenAPI { get; set; }
        public APIManager(AxKHOpenAPILib.AxKHOpenAPI api )
        {
            OpenAPI = api;
        }

        public void SetInputValue( string name, string value )
        {
            OpenAPI.SetInputValue(name, value);
        }

        public int CommRqData( string sRQName, string sTRCode, int nPrevNext, string sScreenNo )
        {
            mut.WaitOne();

            int resultCode = OpenAPI.CommRqData(sRQName, sTRCode, nPrevNext, sScreenNo);
            Thread.Sleep(700);

            mut.ReleaseMutex();

            return resultCode;
        }

        public string GetCommData( string sTrCode, string sRQName, int index, string fieldName )
        {
            return OpenAPI.GetCommData(sTrCode, sRQName, index, fieldName);
        }

        public int CommKwRqData( string sArrCode, int bNext, int nCodeCount, int nTypeFlag, string sRQName, string sScreenNo )
        {
            mut.WaitOne();

            int resultCode = OpenAPI.CommKwRqData(sArrCode, bNext, nCodeCount, nTypeFlag, sRQName, sScreenNo);
            Thread.Sleep(700);

            mut.ReleaseMutex();

            return resultCode;
        }

        public int GetRepeatCnt( string sTrCode, string sRQName )
        {
            return OpenAPI.GetRepeatCnt(sTrCode, sRQName);
        }

        public int SendOrder( string sRQName, string sScreenNo, string sAccNo, int nOrderType, string sCode, int nQty, int nPrice, string sHogaGbn, string sOrgOrderNo)
        {
            mut.WaitOne();

            int resultCode = OpenAPI.SendOrder(sRQName, sScreenNo, sAccNo, nOrderType, sCode, nQty, nPrice, sHogaGbn, sOrgOrderNo);
            Thread.Sleep(700);

            mut.ReleaseMutex();

            return resultCode;
        }

        public int 매도취소요청(string accountNo, string orderSeq , string orgOrderNo, string stockCode, string stockName, string qty, string price)
        {
            log.Info("매도취소요청 start: " + accountNo + " orderSeq:" + orderSeq + " orderNo:" + orgOrderNo + " stockCode:" + stockCode + " stockName:" + stockName
                + " price:" + price.ToString() + " qty:" + qty.ToString());

            // 매도정정요청
            int resultCode = SendOrder("매도취소요청", orderSeq.ToString(), accountNo, 4, stockCode, Util.GetInt(qty), Util.GetInt(price) , "00", orgOrderNo);

            log.Info("매도취소요청 resultCode:" + resultCode.ToString());

            return resultCode;
        }
    }
}
