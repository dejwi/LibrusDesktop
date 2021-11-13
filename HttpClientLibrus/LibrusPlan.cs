using FluentDateTime;
using HtmlAgilityPack;
using HttpClientLibrus.LessonStrucs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientLibrus
{
    public class LibrusPlan
    {
        public List<TimePeriod> timePeriods { get; set; }
        public List<SchoolDay> week { get; set; }

        private const string requestUrl = "https://synergia.librus.pl/przegladaj_plan_lekcji";

        public LibrusPlan(List<TimePeriod> timePeriods, List<SchoolDay> week)
        {
            this.timePeriods = timePeriods;
            this.week = week;
        }

        public static async Task<LibrusPlan> Retrieve(LibrusData connection, IProgress<int> progress,string gWeek = "")
        {
            progress.Report(50);
            //2021-08-30_2021-09-05 gWeek example
            string html;
            if (gWeek != "")
                html = await LibrusUtils.PostReqResponse(requestUrl, connection.cookieSession
                    , $"requestkey=0&tydzien={gWeek}&pokaz_zajecia_zsk=on&pokaz_zajecia_ni=on");
            else
                html = await LibrusUtils.GetReqResponse(requestUrl, connection.cookieSession);
            progress.Report(60);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var document = doc.DocumentNode;

            var table = document.SelectSingleNode("/html/body/div[1]/div/div/div/form/table[2]");
            var rows = table.SelectNodes(".//tr").Skip(1).Reverse().Skip(1).Reverse(); // i just copied it ok

            List<TimePeriod> timePeriods = new List<TimePeriod>();
            List<Lesson>[] lessons = new List<Lesson>[7];
            List<SchoolDay> week = new List<SchoolDay>();

            int unknownPeriods = 0;

            DateTime firstDayOfCurrentWeek = gWeek == "" ? DateTime.Now.BeginningOfWeek() : LibrusUtils.ParseGWeek(gWeek).BeginningOfWeek();

            progress.Report(70);
            foreach (var item in rows)
            {
                var hrNode = item.SelectSingleNode("./*[2]");
                string hrText = LibrusUtils.DeHtmlify(hrNode.InnerText);
                var idNode = item.SelectSingleNode("./*[1]");
                string idText = idNode.InnerText.Trim();

                string sHr = hrText.Split('-')[0].Trim();
                string eHr = hrText.Split('-')[1].Trim();

                var sDate = DateTime.Parse(sHr);
                var eDate = DateTime.Parse(eHr);

                if (idText == "")
                {
                    timePeriods.Add(new TimePeriod(sDate, eDate, -unknownPeriods));
                    unknownPeriods++;
                    continue;
                }

                timePeriods.Add(new TimePeriod(sDate, eDate, int.Parse(idText)));


                string snw = "";
                for (int i = 0; i < 7; i++)
                {
                    DateTime day = firstDayOfCurrentWeek.AddDays(i);

                    if (lessons[i] == null) lessons[i] = new List<Lesson>();
                    var nnn = item.SelectNodes("./*")[i + 2];
                    snw = nnn.InnerText;
                    snw = LibrusUtils.DeHtmlify(snw);
                    bool rep = snw.ToLower().Contains("zastępstwo");
                    bool can = snw.ToLower().Contains("odwołane");

                    var starthour = timePeriods.Last().start;
                    var endhour = timePeriods.Last().end;
                    if (snw == " " || snw == "")
                    {
                        lessons[i].Add(new Lesson("", "", false, false, starthour, endhour, timePeriods.Last().mark));
                        continue;
                    }

                    string tc = snw.Substring(snw.IndexOf('-') + 1, snw.Length - snw.IndexOf('-') - 1);

                    DateTime startDateTime = day.AddHours(starthour.Hour).AddMinutes(starthour.Minute);
                    DateTime endDateTime = day.AddHours(endhour.Hour).AddMinutes(endhour.Minute);
                    lessons[i].Add(new Lesson(
                        LibrusUtils.DeHtmlify(nnn.SelectSingleNode(".//b").InnerText.Trim()).Replace("\n", ""),
                        tc.Trim(), rep, can, startDateTime, endDateTime,
                        timePeriods.Last().mark));
                }
            }
            progress.Report(87);
            int r = 0;
            foreach (var d in lessons)
            {
                r++;
                week.Add(new SchoolDay(d, firstDayOfCurrentWeek.AddDays(r)));
            }

            progress.Report(100);
            //almost all code in this function is coppied sry idk how to reform html data from this site
            return new LibrusPlan(timePeriods, week);
        }
    }
}
