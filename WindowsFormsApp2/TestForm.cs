using log4net.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp2.Common;
using WindowsFormsApp2.Dac;
using WindowsFormsApp2.Data;

namespace WindowsFormsApp2
{
    public partial class TestForm : Form
    {
        public AxKHOpenAPILib.AxKHOpenAPI OpenAPI { get; set; }
        public Biz biz { get; set; }
        public TRBiz trBiz { get; set; }

        DacStock dacStock = new DacStock();

        public TestForm()
        {
            InitializeComponent();
        }



        private void TestForm_Load(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                trBiz.당일거래량상위요청("1", "3");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                trBiz.당일거래량상위요청("3", "3");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                trBiz.전일거래량상위요청("1");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                trBiz.전일거래량상위요청("2");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string 요일 = DateTime.Now.ToString("ddd");

                if (요일.Equals("토") || 요일.Equals("일")) return;

                // 09시부터 15시 30분까지 실행
                if (DateTime.Now.ToString("HHmm").CompareTo("0900") < 0 ||
                    DateTime.Now.ToString("HHmm").CompareTo("1830") > 0) return;


                List<StockTarget> list = dacStock.당일대상조회("20200709", "");

                int pageSize = 50;

                if (list.Count <= pageSize)
                {
                    string stockCodeAr = string.Join(";", list.Select(i => i.stockCode).ToArray());
                    OpenAPI.CommKwRqData(stockCodeAr, 0, list.Count, 0, "종목현재가조회", "1111");

                    Thread.Sleep(500);

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

                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string buySeq = biz.마지막매수주문Seq가져오기("20200717", "005070");

                MessageBox.Show(buySeq);

            } catch ( Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                당일실적 실적 = dacStock.당일실적조회("20200804");

                string message = string.Format(@"들어간금액 : {1} <br/> 실현손익금액 : {2} <br/> 증권사수수료 : {3} <br/> 
                            거래세 : {4} <br/> 현재까지실제수익 : {5} <br/> 보유중평가금액손익 : {6} <br/> 실제예상수익 : {7}"
                            , 실적.매도방식
                            , Util.GetMoneyFormatString(실적.들어간금액)
                            , Util.GetMoneyFormatString(실적.실현손익금액)
                            , Util.GetMoneyFormatString(실적.증권사수수료)
                            , Util.GetMoneyFormatString(실적.거래세)
                            , Util.GetMoneyFormatString(실적.현재까지실제수익)
                            , Util.GetMoneyFormatString(실적.보유중평가금액손익)
                            , Util.GetMoneyFormatString(실적.실제예상수익));

                Util.SendMail("kim2509@gmail.com", "오늘 대용PC 주식매매 결과", message);
                //Util.SendMail("kim2509@gmail.com", "오늘 망구PC 주식매매 결과", message);

            } catch ( Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
