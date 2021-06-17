using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proxy_scraper
{
    enum Type
    {
        Transparant,
        Anonymous,
        Silent
    }

    class Proxy
    {
        public Proxy(string address, int time = 0)
        {
            this.Time = time;
            this.Address = address;
        }

        public int Time;
        public Type Type;
        public string Address;
    }
}
