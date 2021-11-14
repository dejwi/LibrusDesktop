using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LibrusLib
{
    public static class LibrusConnection
    {
        public static async Task<LibrusData> Connect(string username, string password,IProgress<int> progress)
        {
            progress.Report(0);
            var acc = new LibrusData(username, password, new CookieContainer());
            HttpResponseMessage response;
            HttpRequestMessage request;
            string url;
            string iframeCode;

            var client = LibrusUtils.GetClient(ref acc.cookieSession);

            //1
            try
            {
                request = LibrusUtils.GetRequest("https://portal.librus.pl/rodzina/synergia/loguj",
                    "https://portal.librus.pl/rodzina");
                response = await client.SendAsync(request);

                iframeCode = GetIframeCode(await response.Content.ReadAsStringAsync());


                Console.WriteLine("1worked?");
                progress.Report(15);
            }
            catch (Exception)
            {
                Console.WriteLine("1 no ");
                return new LibrusData("FAIL", "FAIL", null);
            }


            //2
            try
            {
                request = LibrusUtils.GetRequest(iframeCode,
                    "https://portal.librus.pl/rodzina");
                response = await client.SendAsync(request);
                url = response.RequestMessage.RequestUri.ToString();
                Console.WriteLine("2worked?");
                progress.Report(25);
            }
            catch (Exception)
            {
                Console.WriteLine("2 no ");
                return new LibrusData("FAIL", "FAIL", null);
            }

            //3
            try
            {
               
                request = LibrusUtils.GetRequest(url); //spend hour fixing this shit, turns out referer header doesnt chagne anything
                                                       //| EDIT: removed it at core from GetRequest() and everything broke nvm
                                                       //GOOD TO KNOW but im not cleaning up rest of the code oh no | EDIT: I am
                LibrusUtils.MakePostRequest(ref request, LibrusUtils.GetContent($"action=login&login={username}&pass={password}"));
                response = await client.SendAsync(request);

                //check if server likes us
                var x = await response.Content.ReadAsStringAsync();
                if (!x.Contains("ok")){
                    Console.WriteLine("3 jebac");
                    return new LibrusData("FAIL", "FAIL", null);
                }

                Console.WriteLine("3worked?");
                progress.Report(35);
            }
            catch (Exception)
            {
                Console.WriteLine("3 no ");
                return new LibrusData("FAIL", "FAIL", null);
            }


            //4
            try
            {
                request = LibrusUtils.GetRequest(url.Replace("Authorization", "Authorization/Grant"),
                    url);
                response = await client.SendAsync(request);
                Console.WriteLine("4worked?");
                progress.Report(50);
            }
            catch (Exception)
            {
                Console.WriteLine("4 no ");
                return new LibrusData("FAIL", "FAIL", null);
            }

            //if worked return
            
            return acc;
        }

        private static string GetIframeCode(string body)
        {
            if (body == String.Empty) throw new Exception("Failed getting Iframe code - body empty");
            HtmlDocument w = new HtmlDocument();
            w.LoadHtml(body);
            string r = w.DocumentNode.SelectSingleNode("//iframe[@id=\"caLoginIframe\"]").GetAttributeValue("src", "ERROR");
            if (r == "ERROR") throw new Exception("Failed getting Iframe code - can't find");
            return r;
        }
    }
}
