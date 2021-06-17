using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;
using Leaf.xNet;

namespace proxy_scraper
{
    static class Parser
    {
        static Mutex mutex = new Mutex();
        static List<string> rawProxies = new List<string>();
        public static List<Proxy> processedProxies = new List<Proxy>();

        public static void Parse(string content)
        {
            List<string> proxies = new List<string>();

            foreach (Match match in new Regex("\\b(\\d{1,3}\\.){3}\\d{1,3}\\:\\d{1,8}\\b", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(content))
            {
                if (match.Value.Count<char>() >= 9)
                {
                    proxies.Add(match.Value);
                }
            }

            var split = Split(proxies.ToArray(), proxies.Count / 100);

            List<Thread> threads = new List<Thread>();

            foreach (var array in split)
            {
                Thread thread = new Thread(new ThreadStart(() => CheckProxyArray(array.ToArray())));

                thread.Start();
                threads.Add(thread);
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        static Proxy ProcessProxy(string rawProxy)
        {
            Proxy proxy = new Proxy(rawProxy);

            string content = Utils.GetWebPageText("http://azenv.net");

            if (content.Contains("HTTP_X_FORWARDED_FOR"))
            {
                proxy.Type = Type.Transparant;
            }

            else if (!content.Contains("HTTP_CLIENT_IP") && !content.Contains("HTTP_X_FORWARDED") && !content.Contains("HTTP_X_CLUSTER_CLIENT_IP") && !content.Contains("HTTP_FORWARDED") && !content.Contains("HTTP_FORWARDED_FOR") && content.Contains("HTTP_VIA"))
            {
                proxy.Type = Type.Anonymous;
            }

            else if (content.Contains("HTTP_") || (content.Contains("HTTP_UPGRADE_INSECURE_REQUESTS") && !content.Contains("HTTP_VIA")))
            {
                proxy.Type = Type.Silent;
            }

            else
            {
                proxy.Type = Type.Transparant;
            }

            string type;

            if (proxy.Type == Type.Anonymous)
            {
                type = "anon";
            }

            else if (proxy.Type == Type.Transparant)
            {
                type = "trans";
            }

            else
            {
                type = "silent";
            }

            Console.WriteLine(proxy.Address + " | " + type);

            return proxy;
        }

        static void CheckProxyArray(string[] proxies)
        {
            foreach (string proxy in proxies)
            {
                if (CheckConnection(proxy))
                {
                    mutex.WaitOne();

                    rawProxies.Add(proxy);

                    File.AppendAllText("proxies.txt", proxy + "\n");

                    mutex.ReleaseMutex();

                    Console.WriteLine(proxy);

                    //ProcessProxy(proxy);
                }

            }
        }

        static Mutex amutex = new Mutex();

        static bool CheckConnection(string proxy)
        {
            mutex.WaitOne();
            foreach (string thisProxy in rawProxies)
            {
                if (thisProxy.Split(':')[0] == proxy.Split(':')[0])
                {
                    mutex.ReleaseMutex();
                    return false;
                }
            }
            mutex.ReleaseMutex();

            try
            {
                HttpRequest request = new HttpRequest();

                request.UseCookies = true;
                request.Referer = "https://twitter.com";

                request.Proxy = new HttpProxyClient(proxy.Split(':')[0], int.Parse(proxy.Split(':')[1]));
                request.Proxy.ConnectTimeout = 3000;
                request.Proxy.ReadWriteTimeout = 3000;

                request.AddHeader("accept", "image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8");
                request.AddHeader("accept-language", "en-US,en;q=0.9");
                request.AddHeader("sec-fetch-dest", "image");
                request.AddHeader("sec-fetch-mode", "no-cors");
                request.AddHeader("sec-fetch-site", "same-origin");
                request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");

                string source = request.Get("https://www.youtube.com/").ToString();

                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }
    }
}
