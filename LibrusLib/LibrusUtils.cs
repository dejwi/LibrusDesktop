using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LibrusLib
{
    public static class LibrusUtils
    {
        public static void SetDefHeadersGet(ref HttpRequestMessage http)
        {
            
            http.Headers.Add("Connection", "keep-alive");
            http.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0");
            http.Headers.Add("Sec-Fetch-Dest", "document");
            http.Headers.Add("Sec-Fetch-Mode", "navigate");
            http.Headers.Add("Sec-Fetch-Site", "same-site");
            http.Headers.Add("Sec-Fetch-User", "?1");
            http.Headers.Add("Sec-GPS", "1");
            http.Headers.Add("DNT", "1");
        }

        public static HttpRequestMessage GetRequest(string url, string referer = "")
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            if (referer != "")
                req.Headers.Referrer = new Uri(referer);

            SetDefHeadersGet(ref req);
            req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");

            return req;
        }

        public static void MakePostRequest(ref HttpRequestMessage request, FormUrlEncodedContent c)
        {
            request.Method = HttpMethod.Post;
            request.Content = c;
            request.Headers.Set("Accept", "*/*");
        }

        public static async Task<string> PostReqResponse(string url, CookieContainer cookies, string content)
        {
            var client = GetClient(ref cookies);
            var request = LibrusUtils.GetRequest(url);
            LibrusUtils.MakePostRequest(ref request, GetContent(content));

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<string> GetReqResponse(string url, CookieContainer cookies)
        {
            var client = GetClient(ref cookies);
            var request = LibrusUtils.GetRequest(url);

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public static HttpClient GetClient(ref CookieContainer cookies)
        {
            return new HttpClient(new HttpClientHandler() { CookieContainer = cookies });
        }

        public static FormUrlEncodedContent GetContent(string c)
        {
            try
            {
                var col = c.Split("&");
                var values = new List<KeyValuePair<string, string>>();
                foreach (var item in col)
                {
                    var x = item.Split("=");
                    values.Add(new KeyValuePair<string, string>(x[0], x[1]));
                }
                return new FormUrlEncodedContent(values);
            }
            catch (Exception)
            {
                Console.WriteLine("error at GetContent");
                return null;
            }
        }

        public static DateTime ParseGWeek(string gWeek)
        {
            //2021-08-30_2021-09-05 example bcs im dumb
            return DateTime.Parse(gWeek.Split('_')[0]);
        }

        public static string DeHtmlify(string h)
        {
            h = h.Replace("&nbsp;", " ");
            h = h.Replace("&nbsp", " ");
            return HttpUtility.HtmlDecode(h);
        }

    }

    public static class HttpRequestHeadersExtensions
    {
        public static void Set(this HttpRequestHeaders headers, string name, string value)
        {
            if (headers.Contains(name)) headers.Remove(name);
            headers.Add(name, value);
        }
    }
}
