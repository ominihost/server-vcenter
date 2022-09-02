using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace OminiHost_Server.Class
{
    class VncProxy
    {
        public int port;
        public string ipDedicated;
        public int portDedicated;
        Process infoprocess;
        Thread threadUpdate;        

        public VncProxy(int port, string ipDedicated, int portDedicated)
        {
            this.port = port;
            this.ipDedicated = ipDedicated;
            this.portDedicated = portDedicated;
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = @"py",
                Arguments = @"proxy\run " + port + " " + ipDedicated + ":" + portDedicated,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            infoprocess = Process.Start(processInfo);

            threadUpdate = new Thread(new ThreadStart(DeleteProcess));
            threadUpdate.Start();
        }
        public void Kill()
        {
            if(infoprocess.HasExited == false)
            {
                infoprocess.Kill();
            }            

            if (threadUpdate != null)
            {
                if(threadUpdate.IsAlive)
                    threadUpdate.Abort();
            }
        }
        private void DeleteProcess()
        {
            Thread.Sleep(120000);
            infoprocess.Kill();
            Thread.Sleep(60000 * 60 * 2);
            Global.proxies.Remove(this);
        }
    }
}
