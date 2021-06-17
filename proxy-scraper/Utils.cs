using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace proxy_scraper
{
    static class Utils
    {
        public static string GetWebPageText(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; rv:78.0) Gecko/20100101 Firefox/78.0";
            request.Timeout = 10000;
            request.ReadWriteTimeout = 12000;
            request.KeepAlive = false;
            request.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            StreamReader streamReader = new StreamReader(responseStream);

            string content = streamReader.ReadToEnd();

            responseStream.Close();
            response.Close();

            return content;
        }
    }
}
