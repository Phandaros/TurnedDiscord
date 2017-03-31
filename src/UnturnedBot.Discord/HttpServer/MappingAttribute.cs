using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnturnedBot.Discord.HttpServer
{
    class EnpointAttribute : Attribute
    {
        public string Map;
        public EnpointAttribute(string s)
        {
            Map = s;
        }
    }
}
