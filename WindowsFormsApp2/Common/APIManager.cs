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
        //private static Mutex mut = new Mutex();
        int delay = 500;

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
            //mut.WaitOne();

            log.Info(string.Format("API호출 CommRqData RQName: {0}, trCode: {1}, screenNo : {2} ", sRQName, sTRCode, sScreenNo ));
            int resultCode = OpenAPI.CommRqData(sRQName, sTRCode, nPrevNext, sScreenNo);

            log.Info("API호출 CommRqData 완료 resultCode : " + resultCode);

            Thread.Sleep(delay);
            

            //mut.ReleaseMutex();

            return resultCode;
        }

        public string GetCommData( string sTrCode, string sRQName, int index, string fieldName )
        {
            return OpenAPI.GetCommData(sTrCode, sRQName, index, fieldName);
        }

        public string GetChejanData(int nFid)
        {
            return OpenAPI.GetChejanData(nFid);
        }

        public int CommKwRqData( string sArrCode, int bNext, int nCodeCount, int nTypeFlag, string sRQName, string sScreenNo )
        {
            //mut.WaitOne();

            log.Info(string.Format("API호출 CommKwRqData RQName: {0}, screenNo : {1} ", sRQName, sScreenNo));
            int resultCode = OpenAPI.CommKwRqData(sArrCode, bNext, nCodeCount, nTypeFlag, sRQName, sScreenNo);

            log.Info("API호출 CommKwRqData 완료 resultCode : " + resultCode);

            Thread.Sleep(delay);

            //mut.ReleaseMutex();

            return resultCode;
        }

        public int GetRepeatCnt( string sTrCode, string sRQName )
        {
            return OpenAPI.GetRepeatCnt(sTrCode, sRQName);
        }

        public int SendOrder( string sRQName, string sScreenNo, string sAccNo, int nOrderType, string sCode, int nQty, int nPrice, string sHogaGbn, string sOrgOrderNo)
        {
            //mut.WaitOne();

            log.Info(string.Format("API호출 SendOrder nOrderType : {0} RQName: {1}, sAccNo: {2}, screenNo : {3}, stockCode: {4}, nQty: {5}, nPrice : {6}, orgOrderNo: {7}"
                , nOrderType.ToString(), sRQName, sAccNo, sScreenNo, sCode, nQty.ToString(), nPrice.ToString(), sOrgOrderNo ));

            int resultCode = OpenAPI.SendOrder(sRQName, sScreenNo, sAccNo, nOrderType, sCode, nQty, nPrice, sHogaGbn, sOrgOrderNo);

            log.Info("API호출완료 SendOrder resultCode:" + resultCode);

            Thread.Sleep(delay);

            //mut.ReleaseMutex();

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
