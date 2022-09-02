using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OminiHost_Server.Data
{
    public class VMEspec
    {
        public string dedicado { get; set; }
        public string templateSystemName { get; set; }
        public string customField { get; set; }
        public string nomeVM { get; set; }
        public long memoriaRam { get; set; }
        public int vCPU { get; set; }
        public int fCPU { get; set; }
        public string novaSenha { get; set; }
        public long HDinKB { get; set; }
        public string ipVM { get; set; }
        public string sistemaOperacional { get; set; }
        public string macAdress { get; set; }
        public string nameDC { get; set; }
        public int CPU_Mhz { get; set; }
        public string NetworkName { get; set; }
        public string HDType { get; set; }

    }
}
