using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace KuyakusyoChecker
{
    public partial class Form1 : Form
    {
        const String citycode = "230010";
        String url = "http://weather.livedoor.com/forecast/webservice/json/v1?city=" + citycode;
        public Form1()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            int count = 0;
            string[] RokuyoLabel = { "大安", "赤口", "先勝", "友引", "先負", "仏滅" };
            var TheDay = dateTimePicker1.Value;
            string DayOfW = TheDay.ToString("dddd");
            int month = TheDay.Month;
            //日を取得する。「30」となる。
            int day = TheDay.Day;
            if ((DayOfW == "土曜日") || (DayOfW == "日曜日"))
            {
                MessageBox.Show(DayOfW + "なのでお休みです");
                return;
            }
            var Today = DateTime.Today;

            string json = new HttpClient().GetStringAsync(url).Result;
            JObject jobj = JObject.Parse(json);
            double interval = (TheDay - Today).TotalDays;
            if((interval >= 0) && (interval <= 2))
            {
                string todayweather = (string)((jobj["forecasts"][(int)interval]["telop"] as JValue).Value);//今日明日明後日の天気の取得
                MessageBox.Show(todayweather);
            }
            

            //旧暦を調べる
            var OldCalender = new JapaneseLunisolarCalendar();
            int OldM = OldCalender.GetMonth(TheDay);
            int OldD = OldCalender.GetDayOfMonth(TheDay);
            
            //閏月を調べる(閏月が無いときは0が返る)
            DateTime UruF = OldCalender.AddDays(TheDay, 1 - OldCalender.GetDayOfYear(TheDay));
            int UruM = OldCalender.GetLeapMonth(OldCalender.GetYear(UruF), OldCalender.GetEra(UruF));

            //旧暦の月の訂正
            if((UruM > 0)&&(OldM >= UruM))
            {
                OldM--;
            }

            string Rokuyo = RokuyoLabel[(OldM + OldD) % 6];

            //多い条件を調べる
            if(Rokuyo == "大安") //婚姻届がたくさん来る
            {
                count++;
            }
            if(DayOfW == "月曜日") //祝日の翌日
            {
                count++;
            }
            if(month == day) //ぞろ目の日
            {
                count++;
            }
            if((month == 12)&&(day == 24))
            {
                count++;
            }
            switch (count)
            {
                case 0:
                    MessageBox.Show("あまり混んでいないと思う");
                    break;
                case 1:
                case 2:
                    MessageBox.Show("かなり混んでいると思う");
                    break;
                default:
                    MessageBox.Show("むっちゃんこ混んでいると思う");
                    break;
            }
            
        }
    }
}
