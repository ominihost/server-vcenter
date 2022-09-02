using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OminiHost_Server.Class;

namespace OminiHost_Server
{
    class Global
    {
        public static readonly string proxyHostIp = "console.ominihost.com";

        private static readonly int portsStart = 1000;
        private static readonly int portsEnd = 2000;

        public static List<VncProxy> proxies = new List<VncProxy>();
        public static TelaLoad telaLoad = null;

        public static int GetFreePort()
        {
            for(int i = portsStart; i < portsEnd; i++)
            {
                bool isPortInUse = false;
                for(int p = 0; p < proxies.Count; p++)
                {
                    VncProxy proxy = proxies[p];                    
                    if(proxy.port == i)
                    {
                        isPortInUse = true;
                        break;
                    }
                }
                if (isPortInUse == false)
                    return i;
            }

            proxies.RemoveAt(0);
            return portsStart;
        }
       
        public static string GenerateRandomCode(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
