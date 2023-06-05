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
        public string IPv6 { get; set; }
        public string IPv6Gateway { get; set; }
        public int IPv6Mask { get; set; }
        public string IPv6DNS1 { get; set; }
        public string IPv6DNS2 { get; set; }
        public string IPv6DNS3 { get; set; }
        public string IPv6DNS4 { get; set; }
        public string IPv4 { get; set; }
        public string IPv4Gateway { get; set; }
        public string IPv4Mask { get; set; }
        public string IPv4DNS1 { get; set; }
        public string IPv4DNS2 { get; set; }
        public string IPv4DNS3 { get; set; }
        public string IPv4DNS4 { get; set; }
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
