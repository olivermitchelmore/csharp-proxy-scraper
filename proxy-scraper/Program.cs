using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace proxy_scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("Press enter to start scraping...");
            Console.ReadLine();

            if (File.Exists("proxies.txt"))
            {
                File.Delete("proxies.txt");
            }

            string[] sources = File.ReadAllLines("sources.txt");

            foreach (string source in sources)
            {
                Console.Clear();
                Console.WriteLine("Scraping " + source);

                string content = Utils.GetWebPageText(source);

                Parser.Parse(content);
            }

            /*foreach (Proxy proxy in Parser.processedProxies)
            {
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
            }*/

            Console.WriteLine("finished");
            Console.ReadLine();
        }
    }
}
